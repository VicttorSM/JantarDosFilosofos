using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;

namespace JantarDosFilosofos.Classes
{
    class Philosopher
    {
        private int id;
        private int timeThinking;
        private int timeEating;
        private List<Fork> forks;
        private List<Fork> forksInHand;
        private Stopwatch _stopwatch;

        public Philosopher(int id, ref List<Fork> forks, ref Stopwatch stopwatch, double timeThinking = 0.01, double timeEating = 0.1)
        {
            this.id = id;
            this.forks = forks;
            this.timeThinking = Convert.ToInt32(timeThinking * 1000);
            this.timeEating = Convert.ToInt32(timeEating * 1000);
            this.forksInHand = new List<Fork>();
            _stopwatch = stopwatch;
        }

        private void Log(string str)
        {
            Console.WriteLine($"{Math.Round(_stopwatch.Elapsed.TotalMilliseconds)}: Philosopher {id} {str}");
        }

        public void Exist(bool startThinking = false)
        {
            Log("sits on the table");
            _stopwatch.Start();
            if (startThinking)
            {
                Think();
            }
            while (true)
            {
                if (!Eat())
                {
                    Hungry();
                }
                else
                {
                    ReleaseForks();
                    Think();
                }
            }
        }


        public void Think()
        {
            Log($"is thinking");
            Thread.Sleep(timeThinking);
        }

        public bool Eat()
        {
            int neededForks = 2;
            for (int i = 0; i < forks.Count && forksInHand.Count < neededForks; i++)
            {
                if (GetFork(i))
                {
                    Log($"got fork {forks[i].GetId()}");
                    forksInHand.Add(forks[i]);
                }
            }

            if (forksInHand.Count >= neededForks)
            {
                Log($"is eating");
                Thread.Sleep(timeEating);
                return true;
            }
            return false;
        }

        public void ReleaseForks()
        {
            foreach (var fork in forksInHand)
            {
                Log($"released fork {fork.GetId()}");
                fork.BePlacedDown();
            }
            forksInHand.Clear();
        }

        public void Die()
        {
            Log($"was not able to eat and DIED");
        }
        
        public void Hungry()
        {
            Log($"is hungry and waiting for forks");
            Stopwatch timeHungry = new Stopwatch();
            timeHungry.Start();
            while (true)
            {
                for (int i = 0; i < forks.Count; i++)
                {
                    if (GetFork(i))
                    {
                        Log($"got fork {forks[i].GetId()}");
                        forksInHand.Add(forks[i]);
                        return;
                    }
                }
            }
        }

        private bool GetFork(int i)
        {
            if (i < 0 || i >= forks.Count)
            {
                throw new IndexOutOfRangeException();
            }
            lock (forks[i])
            {
                return forks[i].BeLifted();
            }
        }

    }
}
