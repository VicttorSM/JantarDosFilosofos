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

        /// <summary>
        /// Creates a Table class that serves as a world to the Dining Philosophers
        /// </summary>
        /// <param name="qtdPhilosophers">Number of philophers in the table</param>
        /// <param name="qtdForks">Number of forks in the table</param>
        /// <param name="timeThinking">Time in seconds that each philosopher will take to think</param>
        /// <param name="timeEating">Time in seconds that each philosopher will take to eat</param>
        /// <param name="universalForkExists">Tells if a fork that every philosopher can use is in the table</param>
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

        /// <summary>
        /// Aborts all the simulations and clears all the Lists
        /// </summary>
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

        /// <summary>
        /// Starts a simulation in the table, and stops when a deadlock happens
        /// </summary>
        /// <param name="maxTime">The time it will wait for a deadlock, after this time the simulation ends</param>
        /// <param name="interval">Interval that each philosopher takes to sit in the table</param>
        /// <param name="timeToClassifyAsDeadLock">The time it takes with all the philosophers hungry to classify as a deadlock</param>
        /// <returns>The time it took to cause a deadlock</returns>
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

        /// <summary>
        /// Verifies if a deadlock has happened and tracks its time
        /// </summary>
        /// <param name="deadlock">Reference variable to tell if the deadlock has happened</param>
        /// <param name="timeLocked">Time the deadlock has been going</param>
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
