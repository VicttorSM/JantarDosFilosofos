using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;

namespace JantarDosFilosofos.Classes
{
    class Table
    {
        private List<Philosopher> philosophers;
        private List<Fork> forks;
        private Stopwatch _stopwatch;
        private List<Thread> threads;

        public Table(int qtdPhilosophers = 5, int qtdForks = 5, double timeThinking = 1, double timeEating = 1, bool universalForkExists = false)
        {
            Fork universalFork = null;
            if (universalForkExists)
            {
                universalFork = new Fork(qtdForks);
            }
            _stopwatch = new Stopwatch();
            philosophers = new List<Philosopher>();
            forks = new List<Fork>();
            threads = new List<Thread>();
            for (int i = 0; i < qtdForks; i++)
            {
                forks.Add(new Fork(i));
            }
            for (int i = 0; i < qtdPhilosophers; i++)
            {
                List<Fork> reachableForks = new List<Fork>();
                if (universalFork != null)
                {
                    reachableForks.Add(universalFork);
                }
                for (int j = 0; j < 2; j++)
                {
                    reachableForks.Add(forks[(i + j) % forks.Count]);
                }
                philosophers.Add(new Philosopher(i, ref reachableForks, ref _stopwatch, timeThinking, timeEating));
            }
            forks.Add(universalFork);
        }

        public void AbortSimulation()
        {
            philosophers.ForEach(x =>
            {
                x.AbortSimulation();
            });
            threads.ForEach(x => x.Join());
            threads.Clear();
            philosophers.Clear();
            forks.Clear();
        }

        public double StartSimulation(TimeSpan maxTime, double interval = 1, double timeToClassifyAsDeadLock = 60)
        {
            int milInterval = Convert.ToInt32(interval * 1000);
            int milTimeToClassifyAsDeadLock = Convert.ToInt32(timeToClassifyAsDeadLock * 1000);

            _stopwatch.Start();
            foreach (var philosopher in philosophers)
            {
                var thread = new Thread(() => philosopher.Exist());
                thread.Start();
                threads.Add(thread);
                Thread.Sleep(milInterval);
            }
            bool deadlock = false;
            Stopwatch timeLocked = new Stopwatch();
            while (true)
            {
                VerifyDeadLock(ref deadlock, ref timeLocked);
                if (timeLocked.ElapsedMilliseconds >= milTimeToClassifyAsDeadLock)
                {
                    break;
                }
                else if (_stopwatch.ElapsedMilliseconds >= maxTime.TotalMilliseconds)
                {
                    AbortSimulation();
                    return -1;
                }
            }
            double timeToDeadLock = philosophers.Select(x => x.lastTimeHungry).Max();
            AbortSimulation();
            return timeToDeadLock;
        }

        private void VerifyDeadLock(ref bool deadlock, ref Stopwatch timeLocked)
        {
            if (!deadlock)
            {
                timeLocked.Reset();
            }
            else
            {
                timeLocked.Start();
            }
            foreach (var philosopher in philosophers)
            {
                if (!philosopher.IsHungry())
                {
                    deadlock = false;
                    return;
                }
            }
            deadlock = true;
        }
    }
}
