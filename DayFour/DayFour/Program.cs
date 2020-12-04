using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DayFour
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            List<PassportModel> passportModels;

            using (var file = File.OpenRead("PuzzleInput.txt"))
            using (var reader = new StreamReader(file))
            {
                var parser = new InputParser();

                passportModels = parser.ParseFile(reader);
            }

            var partOneValidator = new PassportValidator(new List<string>
                                                         {
                                                             "byr",
                                                             "iyr",
                                                             "eyr",
                                                             "hgt",
                                                             "hcl",
                                                             "ecl",
                                                             "pid"
                                                         });

            var partOneValidPassports = passportModels.Count(partOneValidator.IsPassportValid);

            Console.WriteLine($"Part one valid passports: {partOneValidPassports}.");

            Console.ReadKey();
        }

        #endregion
    }
}