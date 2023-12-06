using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day06 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            return races.Select(GetWaysToWinRace)
                .Aggregate(1, (agg, next) => agg * next)
                .ToString();
        }

        private int GetWaysToWinRace(Race race)
        {
            // Making a guess / assumption here that the amount of time to hold the button is continuous
            // So we can just search for the min & max values of time where holding the button beats the record
            var lowestWinningHoldTime = default(int?);

            for (var holdTime = 1; holdTime < race.Time; holdTime++)
            {
                var timeRemaining = race.Time - holdTime;
                var distanceMoved = holdTime * timeRemaining;

                if (distanceMoved > race.DistanceRecord)
                {
                    lowestWinningHoldTime = holdTime;
                    break;
                }
            }

            var highestWinningHoldTime = default(int?);

            for (var holdTime = race.Time - 1; holdTime > 0; holdTime--)
            {
                var timeRemaining = race.Time - holdTime;
                var distanceMoved = holdTime * timeRemaining;

                if (distanceMoved > race.DistanceRecord)
                {
                    highestWinningHoldTime = holdTime;
                    break;
                }
            }

            if (lowestWinningHoldTime is null || highestWinningHoldTime is null)
            {
                throw new Exception("Did not find result for race");
            }

            return highestWinningHoldTime.Value - lowestWinningHoldTime.Value + 1;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        public override async Task ReadInput()
        {
            var strings = await new StringFileReader().ReadInputFromFile();

            var times = strings[0]
                .Split(':')[1]
                .Split()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(int.Parse)
                .ToArray();

            var distances = strings[1]
                .Split(':')[1]
                .Split()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(int.Parse)
                .ToArray();

            races = new List<Race>(times.Length);

            for (var i = 0; i < times.Length; i++)
            {
                races.Add(new Race
                {
                    Time = times[i],
                    DistanceRecord = distances[i]
                });
            }
        }

        private List<Race> races;

        class Race
        {
            public int Time { get; set; }

            public int DistanceRecord { get; set; }
        }
    }
}
