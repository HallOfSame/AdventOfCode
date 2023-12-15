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
            var boxes = Enumerable.Range(0, 256)
                .Select(x => new LensBox(x))
                .ToDictionary(x => x.Id, x => x);

            foreach (var command in specialStrings)
            {
                if (command.Value.EndsWith('-'))
                {
                    // Remove command
                    var lensLabel = new SpecialString
                    {
                        Value = command.Value[..^1],
                    };

                    var lens = new Lens(lensLabel, 0);

                    var boxId = lensLabel.GetHashCode();

                    boxes[boxId].RemoveLens(lens);
                }
                else
                {
                    // Add command
                    var lensSplit = command.Value.Split('=');
                    var lensLabel = new SpecialString
                    {
                        Value = lensSplit[0]
                    };
                    var focalPower = int.Parse(lensSplit[1]);
                    var lens = new Lens(lensLabel, focalPower);

                    var boxId = lensLabel.GetHashCode();
                    boxes[boxId]
                        .AddOrUpdateLens(lens);
                }
            }

            var result = boxes.Values.Select(x => x.FocusingPower())
                .Sum();

            return result.ToString();
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

        class LensBox
        {
            public int Id { get; }

            private readonly List<Lens> lensList;

            public LensBox(int id)
            {
                Id = id;
                lensList = new List<Lens>();
            }

            public void RemoveLens(Lens lensToRemove)
            {
                var removed = lensList.Remove(lensToRemove);

                // Console.WriteLine($"Removed {lensToRemove.Label.Value} from box {Id}... Present? {removed}");
            }

            public void AddOrUpdateLens(Lens lensToAdd)
            {
                var existingIndex = lensList.IndexOf(lensToAdd);

                if (existingIndex >= 0)
                {
                    // Console.WriteLine($"Overwrite lens {lensToAdd.Label.Value} in box {Id} index {existingIndex} from power {lensList[existingIndex].FocalPower} to {lensToAdd.FocalPower}");

                    lensList[existingIndex] = lensToAdd;
                    return;
                }

                // Console.WriteLine($"Added lens {lensToAdd.Label.Value} in box {Id} with {lensToAdd.FocalPower} to end of list");

                lensList.Add(lensToAdd);
            }

            public int FocusingPower()
            {
                var boxModifier = Id + 1;

                var lensPowers = lensList.Select((lens, idx) => lens.FocalPower * (idx + 1) * boxModifier);

                return lensPowers.Sum();
            }
        }

        class Lens
        {
            public SpecialString Label { get; }
            public int FocalPower { get; }

            public Lens(SpecialString label, int focalPower)
            {
                Label = label;
                FocalPower = focalPower;
            }

            public override bool Equals(object? obj)
            {
                return ((Lens)obj).Label.Value == this.Label.Value;
            }

            public override int GetHashCode()
            {
                return Label.GetHashCode();
            }
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
