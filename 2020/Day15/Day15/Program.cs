using System;
using System.Collections.Generic;
using System.Linq;

namespace Day15
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            var input = "";

            var listOfNumbers = new Queue<int>(input.Split(',')
                                                    .Select(int.Parse)
                                                    .ToList());

            var turn = 1;

            var numberSpoken = 0;

            var memory = new Dictionary<int, LimitedQueue<int>>();

            var neededNumber = 30000000;

            while (turn <= neededNumber)
            {
                if (listOfNumbers.Any())
                {
                    numberSpoken = listOfNumbers.Dequeue();
                }
                else
                {
                    if (!memory.TryGetValue(numberSpoken,
                                            out var turnInfo))
                    {
                        numberSpoken = 0;
                    }
                    else
                    {
                        if (turnInfo.Count == 1)
                        {
                            numberSpoken = 0;
                        }
                        else
                        {
                            numberSpoken = (turn - 1) - turnInfo.Peek();
                        }
                    }
                }

                if (!memory.ContainsKey(numberSpoken))
                {
                    var newQueue = new LimitedQueue<int>(2);

                    newQueue.Enqueue(turn);

                    memory[numberSpoken] = newQueue;
                }
                else
                {
                    memory[numberSpoken]
                        .Enqueue(turn);
                }

                // Slow
                //Console.WriteLine($"Turn {turn} number {numberSpoken}.");

                turn++;
            }

            Console.WriteLine($"Last word spoken: {numberSpoken}.");
        }

        #endregion
    }

    public class LimitedQueue<T> : Queue<T>
    {
        public int Limit { get; set; }

        public LimitedQueue(int limit) : base(limit)
        {
            Limit = limit;
        }

        public new void Enqueue(T item)
        {
            while (Count >= Limit)
            {
                Dequeue();
            }
            base.Enqueue(item);
        }
    }
}