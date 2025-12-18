using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Helpers.Drawing;
using Helpers.Extensions;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day09 : SingleExecutionPuzzle<Day09.State>
    {
        public override PuzzleInfo Info => new(2025, 9, "Movie Theater");

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var max = 0m;

            foreach (var combo in InitialState.RedTiles.Combinations(2))
            {
                var cornerOne = combo[0];
                var cornerTwo = combo[1];

                // +1 since the corners are included
                var width = Math.Abs(cornerOne.X - cornerTwo.X) + 1;
                var height = Math.Abs(cornerOne.Y - cornerTwo.Y) + 1;

                var area = width * height;
                max = Math.Max(max, area);
            }

            return max.ToString(CultureInfo.InvariantCulture);
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }

        protected override async Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
        {
            var redTiles = puzzleInput.Split('\n')
                .Select(x =>
                {
                    var split = x.Split(',');
                    return new Coordinate(int.Parse(split[0]), int.Parse(split[1]));
                });

            return new State
            {
                RedTiles = redTiles.ToHashSet()
            };
        }

        public DrawableCoordinate[] GetCoordinates()
        {
            throw new NotImplementedException();
        }

        public class State
        {
            public required HashSet<Coordinate> RedTiles { get; set; }
        }
    }
}
