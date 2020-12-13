using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day13
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            var fileText = File.ReadAllText("PuzzleInput.txt");

            var fileSplit = fileText.Split(Environment.NewLine);

            var timeYouCanDepart = long.Parse(fileSplit[0]);

            var availableBuses = fileSplit[1]
                                 .Split(',')
                                 .ToList();

            var busList = new List<Bus>();

            foreach (var bus in availableBuses)
            {
                if (bus == "x")
                {
                    busList.Add(new Bus(0,
                                        -1));
                    continue;
                }

                var busId = long.Parse(bus);

                var tempTime = timeYouCanDepart;

                Math.DivRem(tempTime,
                            busId,
                            out var remainder);

                tempTime = tempTime + (busId - remainder);

                busList.Add(new Bus(tempTime,
                                    busId));
            }

            // PT 1

            var busYouCanTake = busList.Where(x => x.Id != -1)
                                       .OrderBy(x => x.NextDeparture - timeYouCanDepart)
                                       .First();

            var waitTime = busYouCanTake.NextDeparture - timeYouCanDepart;

            Console.WriteLine($"You can take bus ID {busYouCanTake.Id} after waiting {waitTime} min. Puzzle result: {busYouCanTake.Id * waitTime}.");

            // PT 2

            bool IsMultipleOf(long time,
                              long busId)
            {
                return time % busId == 0;
            }

            var currentTime = 0L;

            var timeIncrement = busList[0]
                .Id;

            // Starting at the bus after the first
            for (var i = 1; i < busList.Count; i++)
            {
                var bus = busList[i];

                if (bus.Id == -1)
                {
                    continue;
                }

                while (true)
                {
                    // Keep moving up by prior increment
                    currentTime += timeIncrement;

                    if (IsMultipleOf(currentTime + i,
                                     bus.Id))
                    {
                        // When this matches our constraint we can start incrementing by the next time these two will line up like this
                        // As we get more buses this accelerates the calculation
                        timeIncrement *= bus.Id;
                        break;
                    }
                }
            }

            Console.WriteLine($"Found the required time at {currentTime}.");
        }

        #endregion
    }
}