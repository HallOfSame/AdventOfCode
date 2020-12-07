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

            var bagTree = new BagTreeFactory().BuildTree(models);

            var desiredColor = "shiny gold";

            Console.WriteLine($"{desiredColor} can be in {bagTree.GetAllowedParentColorsForColor(desiredColor).Count} different bags.");

            Console.WriteLine($"A {desiredColor} bag requires {bagTree.GetNumberOfBagsRequiredForColor(desiredColor)} bags inside of it.");
        }

        #endregion
    }
}