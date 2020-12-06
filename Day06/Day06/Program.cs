using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Day06
{
    internal class Program
    {
        #region Class Methods

        public static async Task Main(string[] args)
        {
            List<GroupModel> groupModels;

            await using (var file = File.OpenRead("PuzzleInput.txt"))
            {
                using var reader = new StreamReader(file);
                groupModels = await new GroupParser().GetGroupModels(reader);
            }

            // Part One
            var numberOfQuestionsAnsweredYesTotal = groupModels.Sum(x => x.NumberOfQuestionsAnsweredYesByAny);

            Console.WriteLine($"Number of questions answered yes summed: {numberOfQuestionsAnsweredYesTotal}.");

            // Part Two

            var numberOfQuestionsAnsweredYesIntersect = groupModels.Sum(x => x.NumberOfQuestionsAnsweredYesByAll);

            Console.WriteLine($"Number of questions answered yes by all group members {numberOfQuestionsAnsweredYesIntersect}.");
        }

        #endregion
    }
}