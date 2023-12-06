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
                .Aggregate(1L, (agg, next) => agg * next)
                .ToString();
        }

        private long GetWaysToWinRace(Race race)
        {
            // Making a guess / assumption here that the amount of time to hold the button is continuous
            // So we can just search for the min & max values of time where holding the button beats the record
            var lowestWinningHoldTime = default(long?);

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

            var highestWinningHoldTime = default(long?);

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
            var actualTime = long.Parse(races.Select(x => x.Time).Aggregate(string.Empty, (agg, val) => agg + val));
            var actualDistance = long.Parse(races.Select(x => x.DistanceRecord).Aggregate(string.Empty, (agg, val) => agg + val));

            return GetWaysToWinRace(new Race
                {
                    Time = actualTime,
                    DistanceRecord = actualDistance,
                })
                .ToString();
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
            public long Time { get; set; }

            public long DistanceRecord { get; set; }
        }
    }
}
