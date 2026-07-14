using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace task17
{
    public interface ICommand
    {
        void Execute();
    }
    public interface IScheduler
    {
        bool HasCommand();
        ICommand Select();
        void Add(ICommand command);
    }
    public class RoundRobinScheduler : IScheduler
    {
        private readonly Queue<ICommand> _tasks = new Queue<ICommand>();
        public bool HasCommand() => _tasks.Count > 0;
        public ICommand Select() => _tasks.Dequeue();
        public void Add(ICommand command) => _tasks.Enqueue(command);
    }
    public class ServerThread
    {
        private readonly BlockingCollection<ICommand> _queue = new BlockingCollection<ICommand>();
        private readonly IScheduler _scheduler;
        private readonly Thread _thread;

        private bool _stopRequested = false;
        private Action _currentBehavior;

        public ServerThread(IScheduler scheduler)
        {
            _scheduler = scheduler;
            _currentBehavior = NormalBehavior;
            _thread = new Thread(() =>
            {
                while (!_stopRequested)
                {
                    _currentBehavior();
                }
            });
        }
        public void Start() => _thread.Start();
        public void Enqueue(ICommand cmd) => _queue.Add(cmd);
        public int ManagedThreadId => _thread.ManagedThreadId;
        private void NormalBehavior()
        {
            ICommand cmd = null;

            if (_scheduler.HasCommand())
            {
                if(!_queue.TryTake(out cmd))
                {
                    cmd = _scheduler.Select();
                }
            }
            else
            {
                try
                {
                    cmd = _queue.Take();
                }
                catch (InvalidOperationException)
                { 
                    return;
                }
            }
            SafeExecute(cmd);
        }

        private void SoftStopBehavior()
        {
            if (_queue.TryTake(out var cmd))
            {
                SafeExecute(cmd);
            }
            else if (_scheduler.HasCommand())
            {
                SafeExecute(_scheduler.Select());
            }
            else
            {
                _stopRequested = true;
            }
        }

        private void SafeExecute(ICommand cmd)
        {
            if (cmd == null) return;
            try
            {
                cmd.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void RequestHardStop()
        {
            CheckThreadAccess("HardStop");
            _stopRequested = true;
        }
        public void RequestSoftStop()
        {
            CheckThreadAccess("SoftStop");
            _currentBehavior = SoftStopBehavior;
        }

        private void CheckThreadAccess(string cmdName)
        {
            if (Thread.CurrentThread.ManagedThreadId != ManagedThreadId)
            {
                throw new InvalidOperationException($"Команда {cmdName} может быть вызвана только внутри самого потока сервера");
            }
        }
    }

    public class HardStopCommand : ICommand
    {
        private readonly ServerThread _server;
        public HardStopCommand(ServerThread server) => _server = server;
        public void Execute() => _server.RequestHardStop();
    }

    public class SoftStopCommand : ICommand
    {
        private readonly ServerThread _server;
        public SoftStopCommand(ServerThread server) => _server = server;
        public void Execute() => _server.RequestSoftStop();
    }

    public class TestCommand : ICommand
    {
        private readonly int _id;
        private readonly int _targetCount;
        private readonly IScheduler _scheduler;
        private int _counter = 0;
        public TestCommand(int id, int targetCount, IScheduler scheduler)
        {
            _id = id;
            _targetCount = targetCount;
            _scheduler = scheduler;
        }

        public void Execute()
        {
            _counter++;
            Console.WriteLine($"Поток {_id} вызов {_counter}");
            if (_counter < _targetCount)
            {
                _scheduler.Add(this);
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var scheduler = new RoundRobinScheduler();
            var server = new ServerThread(scheduler);

            server.Start();
            Console.WriteLine("Добавление 5 экземпляров TestCommand в очередь");

            for (int i = 0; i < 5; i++)
            {
                server.Enqueue(new TestCommand(i, 3, scheduler));
            }
            Thread.Sleep(2000);
            Console.WriteLine("Отправка команды HardStop");
            server.Enqueue(new HardStopCommand(server));

            Thread.Sleep(500);
        }
    }
}
