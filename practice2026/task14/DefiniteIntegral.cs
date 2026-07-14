using System;
using System.Threading;

namespace task14
{
    public class DefiniteIntegral
    {
        public static double SolveSingleThread(double a, double b, Func<double,double> function, double step)
        {
            double sum = 0.0;
            for (double x = a; x < b; x += step)
            {
                sum += function(x + step / 2.0) * step;
            }
            return sum;
        }
        public static double Solve(double a, double b, Func<double, double> function, double step, int threadsnumber)
        {
            double totalIntegral = 0.0;
            using (Barrier barrier = new Barrier(threadsnumber + 1))
            {
                double intervalLength = (b-a)/threadsnumber;
                for (int i = 0; i < threadsnumber; i++)
                {
                    int threadIndex = i;
                    Thread t = new Thread(() =>
                    {
                        double localSum = 0.0;
                        double start = a + threadIndex*intervalLength;
                        double end = (threadIndex == threadsnumber - 1) ? b : start + intervalLength;
                        int localSteps = (int)Math.Ceiling((end - start) / step);

                        for (int k = 0; k < localSteps; k++)
                        {
                            double x1 = start + k * step;
                            double x2 = Math.Min(start + (k + 1) * step, end);
                            double dx = x2 - x1;

                            localSum += (function(x1) + function(x2)) / 2.0 * dx;
                        }

                        InterlockedAdd(ref totalIntegral, localSum);
                        barrier.SignalAndWait();
                    });
                    t.Start();
                }
                barrier.SignalAndWait();
            }
            return totalIntegral;
        }
        private static void InterlockedAdd(ref double location, double value)
        {
            double current = location;
            while (true)
            {
                double newValue = current + value;
                double original = Interlocked.CompareExchange(ref location, newValue, current);
                if (original == current)
                {
                    break;
                }
                current = original;
            }
        }
    }
}
