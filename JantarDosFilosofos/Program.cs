using JantarDosFilosofos.Classes;
using System;

namespace JantarDosFilosofos
{
    class Program
    {
        static void RealizaTestes(string fullFilePath, bool universalFork = false, int qtdPhilosophers = 5, int qtdForks = 5, double tComendo = 1)
        {
            for (double i = 0.1; i <= 1; i += 0.1)
            {
                var tPensando = tComendo * i;
                var table = new Table(
                    qtdPhilosophers: qtdPhilosophers,
                    qtdForks: qtdForks,
                    timeThinking: tPensando,
                    timeEating: tComendo,
                    universalForkExists: universalFork);
                double time = table.StartSimulation(new TimeSpan(0, 2, 0), 0, 10);
                Console.WriteLine($"DeadLock in {time}");
                var msg = $@"
philosophers: {qtdPhilosophers}
forks: {qtdForks}
universalFork: {universalFork}
tPensando: {tPensando}
tComendo: {tComendo}
deadlock: {time}
";
                System.IO.File.AppendAllText(fullFilePath, msg);
            }
        }
        static void Main(string[] args)
        {
            var fullFilePath = @"C:\Users\Public\Documents\JantarDosFilosofos.txt";
            System.IO.File.WriteAllText(fullFilePath, ""); // Cleans the file

            RealizaTestes(fullFilePath);
            RealizaTestes(fullFilePath, qtdPhilosophers: 7);
            RealizaTestes(fullFilePath, universalFork: true);
            Console.WriteLine("Finalizado...");
            Console.ReadLine();
        }
    }
}
