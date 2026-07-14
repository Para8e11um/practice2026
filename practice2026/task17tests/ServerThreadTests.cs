using System.Threading;
using System.Collections.Generic;
using task17;
using Xunit;

public class ServerThreadTests
{
    private class ActionCommand : ICommand
    {
        private readonly Action _action;
        public ActionCommand(Action action) { _action = action; }
        public void Execute()
        {
            _action();
        }
    }

    [Fact]
    public void StopCommands_FromExternalThread()
    {
        var server = new ServerThread(new RoundRobinScheduler());
        server.Start();

        var ex = Assert.Throws<InvalidOperationException>(() => server.RequestSoftStop());
        Assert.Contains("только внутри самого потока", ex.Message);
    }

    [Fact]
    public void HardStop_IgnoresCommands()
    {
        var server = new ServerThread(new RoundRobinScheduler());
        server.Start();
        bool executed = false;

        server.Enqueue(new HardStopCommand(server));
        server.Enqueue(new ActionCommand(() => executed = true));

        Thread.Sleep(200);
        Assert.False(executed);
    }

    [Fact]
    public void SoftStop_FinishesCommands()
    {
        var server = new ServerThread(new RoundRobinScheduler());
        server.Start();
        bool executed = false;

        server.Enqueue(new SoftStopCommand(server));
        server.Enqueue(new ActionCommand(()=>executed= true));

        Thread.Sleep(200);
        Assert.True(executed);
    }
    private class MockCommand : ICommand
    {
        public bool IsExecuted { get; private set; }
        public void Execute() => IsExecuted = true;
    }
    private class TraceCommand : ICommand
    {
        private readonly string _name;
        private readonly int _targetCount;
        private readonly IScheduler _scheduler;
        private readonly List<string> _executionTrace;
        private readonly int _workDelayMs;
        private int _currentCount = 0;
        public TraceCommand(string name, int targetCount, IScheduler scheduler, List<string> executionTrace, int workDelayMs)
        {
            _name = name;
            _targetCount = targetCount;
            _scheduler = scheduler;
            _executionTrace = executionTrace;
            _workDelayMs = workDelayMs;
        }
        public void Execute()
        {
            _currentCount++;
            lock (_executionTrace)
            {
                _executionTrace.Add($"{_name}-{_currentCount}");
            }
            if (_workDelayMs > 0)
            {
                Thread.Sleep(_workDelayMs);
            }
            if (_currentCount < _targetCount)
            {
                _scheduler.Add(this);
            }
        }
    }
    [Fact]
    public void RoundRobinScheduler_AddsAndSelectsCommands_InFifoOrder()
    {
        var scheduler = new RoundRobinScheduler();
        var cmd1 = new MockCommand();
        var cmd2 = new MockCommand();
        scheduler.Add(cmd1);
        scheduler.Add(cmd2);
        Assert.True(scheduler.HasCommand());
        Assert.Equal(cmd1, scheduler.Select());
        Assert.Equal(cmd2, scheduler.Select());
        Assert.False(scheduler.HasCommand());
    }
    [Fact]
    public void ServerThread_ExecutesLongTasks_PseudoParallely()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        var executionTrace = new List<string>();
        server.Start();
        server.Enqueue(new TraceCommand("A", 2, scheduler, executionTrace,0));
        server.Enqueue(new TraceCommand("B", 2, scheduler, executionTrace,0));
        Thread.Sleep(300);
        server.Enqueue(new HardStopCommand(server));
        Thread.Sleep(100);
        lock (executionTrace)
        {
            Assert.Equal(4, executionTrace.Count);
            Assert.Equal("A-1", executionTrace[0]);
            Assert.Equal("B-1", executionTrace[1]);
            Assert.Equal("A-2", executionTrace[2]);
            Assert.Equal("B-2", executionTrace[3]);
        }
    }
    [Fact]
    public void ServerThread_PrioritizesNewCommandsFromQueue_OverScheduler()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        var executionTrace = new List<string>();
        server.Start();
        server.Enqueue(new TraceCommand("LongTask", 100, scheduler, executionTrace,10));
        Thread.Sleep(50);
        server.Enqueue(new TraceCommand("FastTask", 1, scheduler, executionTrace, 0));
        Thread.Sleep(100);
        server.Enqueue(new HardStopCommand(server));
        Thread.Sleep(100);
        lock (executionTrace)
        {
            bool fastTaskExecuted = executionTrace.Contains("FastTask-1");
            Assert.True(fastTaskExecuted, "Новая команда из очереди должна обрабатываться даже при наличии задач в планировщике.");
            int fastTaskIndex = executionTrace.IndexOf("FastTask-1");
            Assert.True(fastTaskIndex > 0 && fastTaskIndex < 100, "FastTask должна выполниться между итерациями LongTask.");
        }
    }
}

