using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Table(int qtdPhilosophers = 5, int qtdForks = 5)
        {
            _stopwatch = new Stopwatch();
            philosophers = new List<Philosopher>();
            forks = new List<Fork>();
            for (int i = 0; i < qtdForks; i++)
            {
                forks.Add(new Fork(i));
            }
            for (int i = 0; i < qtdPhilosophers; i++)
            {
                List<Fork> reachableForks = new List<Fork>();
                for (int j = 0; j < 2; j++)
                {
                    reachableForks.Add(forks[(i + j) % forks.Count]);
                }
                philosophers.Add(new Philosopher(i, ref reachableForks, ref _stopwatch));
            }
        }

        public void StartSimulation(double interval = 1)
        {
            int milInterval = Convert.ToInt32(interval * 1000);
            _stopwatch.Start();
            List<Thread> threads = new List<Thread>();
            foreach (var philosopher in philosophers)
            {
                var thread = new Thread(() => philosopher.Exist());
                thread.Start();
                threads.Add(thread);
                Thread.Sleep(milInterval);
            }
            threads.ForEach(t => t.Join());
        }
    }
}
