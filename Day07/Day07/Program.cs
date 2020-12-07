using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Day07
{
    public class Program
    {
        #region Class Methods

        public static async Task Main(string[] args)
        {
            List<BagRuleModel> models;

            await using (var file = File.OpenRead("PuzzleInput.txt"))
            {
                using var streamReader = new StreamReader(file);

                models = await new RuleParser().GetBagRulesFromInput(streamReader);
            }

            var container = new RuleContainer(models);

            var desiredColor = "shiny gold";

            var outerBags = container.GetOuterColorsAllowingBag(desiredColor);

            Console.WriteLine($"{desiredColor} can be in {outerBags.Count} different bags.");
        }

        #endregion
    }
}