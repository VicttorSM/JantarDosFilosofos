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

        /// <summary>
        /// Creates a new Philosopher class
        /// </summary>
        /// <param name="id">Identity of the philosopher</param>
        /// <param name="forks">Forks this philosopher can reach</param>
        /// <param name="stopwatch">Shared time with the other philosophers in the table</param>
        /// <param name="timeThinking">Time in seconds this philosopher takes to think</param>
        /// <param name="timeEating">Time in seconds this philosopher takes to eat</param>
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

        /// <summary>
        /// Class that allows other classes to abort the simulation of this philosopher if a deadlock happens
        /// </summary>
        public void AbortSimulation()
        {
            abortSimulation = true;
        }
        

        /// <summary>
        /// Logs an action of the philosopher in the terminal
        /// </summary>
        /// <param name="str">Action of the philosopher</param>
        private void Log(string str)
        {
            Console.WriteLine($"{Math.Round(_stopwatch.Elapsed.TotalMilliseconds)}: Philosopher {id} {str}");
        }

        /// <summary>
        /// Starts a simulation of a philosopher
        /// </summary>
        /// <param name="startThinking">If this variabel is true, then the philosopher will start the simulation by thinking</param>
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

        /// <summary>
        /// Philosopher thinks
        /// </summary>
        public void Think()
        {
            Log($"is thinking");
            Thread.Sleep(timeThinking);
        }

        /// <summary>
        /// Philosopher tries to eat
        /// </summary>
        /// <returns>True if he was able to eat, false if he was not</returns>
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

        /// <summary>
        /// Releases all forks the philosopher has in hands
        /// </summary>
        public void ReleaseForks()
        {
            foreach (var fork in forksInHand)
            {
                Log($"released fork {fork.GetId()}");
                fork.BePlacedDown();
            }
            forksInHand.Clear();
        }

        /// <summary>
        /// Philosopher dies
        /// </summary>
        public void Die()
        {
            Log($"was not able to eat and DIED");
            AbortSimulation();
        }

        /// <summary>
        /// Returns if the philosopher is hungry and waiting for forks to eat
        /// </summary>
        /// <returns>True if the philosopher is hungry, false otherwise</returns>
        public bool IsHungry()
        {
            return hungry;
        }

        /// <summary>
        /// Philosopher is hungry and keeps checking to see if enough forks are available in order for him to eat
        /// </summary>
        /// <returns>True if the philosopher was able to eat, false if the simulation was aborted</returns>
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
        
        /// <summary>
        /// Tries to get a fork
        /// </summary>
        /// <param name="i">Id of the fork the philosopher tried to grab</param>
        /// <returns>True if the philosopher was able to pick up the fork, false otherwise</returns>
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
