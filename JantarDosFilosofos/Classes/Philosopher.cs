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
        private bool hungry;
        private bool abortSimulation;
        public double lastTimeHungry;

        public Philosopher(int id, ref List<Fork> forks, ref Stopwatch stopwatch, double timeThinking = 0.01, double timeEating = 0.1)
        {
            this.id = id;
            this.forks = forks;
            this.timeThinking = Convert.ToInt32(timeThinking * 1000);
            this.timeEating = Convert.ToInt32(timeEating * 1000);
            this.forksInHand = new List<Fork>();
            this.hungry = false;
            this.abortSimulation = false;
            this.lastTimeHungry = 0;
            _stopwatch = stopwatch;
        }

        public void AbortSimulation()
        {
            abortSimulation = true;
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
                if (abortSimulation)
                    return;
                if (!Eat())
                {
                    if(!Hungry())
                    {
                        return;
                    }
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

        public bool IsHungry()
        {
            return hungry;
        }
        
        public bool Hungry()
        {
            Log($"is hungry and waiting for forks");
            lastTimeHungry = _stopwatch.ElapsedMilliseconds;
            hungry = true;
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
                        hungry = false;
                        return true;
                    }
                    else if (abortSimulation)
                    {
                        return false;
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
