using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day15 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var result = specialStrings.Select(x => x.GetHashCode())
                .Sum();

            return result.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        private List<SpecialString> specialStrings;

        public override async Task ReadInput()
        {
            var strings = await new StringFileReader().ReadInputFromFile();

            // Puzzle input says to ignore newlines so check for them
            specialStrings = strings.SelectMany(x => x.Split(','))
                .Select(x => new SpecialString
                {
                    Value = x
                })
                .ToList();
        }

        class SpecialString
        {
            public string Value { get; set; }

            public override int GetHashCode()
            {
                var currentValue = 0;

                for (var i = 0; i < Value.Length; i++)
                {
                    var currentChar = (int)Value[i];

                    currentValue += currentChar;
                    currentValue *= 17;
                    currentValue %= 256;
                }

                return currentValue;
            }
        }
    }
}
