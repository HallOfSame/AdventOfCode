using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileText = File.ReadAllText("PuzzleInput.txt");

            var fileSplit = fileText.Split(Environment.NewLine);

            var timeYouCanDepart = int.Parse(fileSplit[0]);

            var availableBuses = fileSplit[1]
                                 .Split(',')
                                 .Where(x => x != "x")
                                 .Select(int.Parse)
                                 .ToList();

            var busList = new List<Bus>();

            foreach (var busId in availableBuses)
            {
                var tempTime = timeYouCanDepart;


                Math.DivRem(tempTime,
                            busId,
                            out var remainder);

                tempTime = tempTime + (busId - remainder);

                busList.Add(new Bus(tempTime,
                                    busId));
            }

            // PT 1

            var busYouCanTake = busList.OrderBy(x => x.NextDeparture - timeYouCanDepart)
                                       .First();

            var waitTime = busYouCanTake.NextDeparture - timeYouCanDepart;

            Console.WriteLine($"You can take bus ID {busYouCanTake.Id} after waiting {waitTime} min. Puzzle result: {busYouCanTake.Id * waitTime}.");
        }
    }
}
