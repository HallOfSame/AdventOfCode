using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
                                                         }.Select(x => new FieldValidator(x,
                                                                                          (s) => true))
                                                          .ToList());

            var partOneValidPassports = passportModels.Count(partOneValidator.IsPassportValid);

            Console.WriteLine($"Part one valid passports: {partOneValidPassports}.");

            var byrValidator = new FieldValidator("byr",
                                                  x =>
                                                  {
                                                      if (!int.TryParse(x,
                                                                        out var xInt))
                                                      {
                                                          return false;
                                                      }

                                                      return xInt >= 1920 && xInt <= 2002;
                                                  });

            var iyrValidator = new FieldValidator("iyr",
                                                  x =>
                                                  {
                                                      if (!int.TryParse(x,
                                                                        out var xInt))
                                                      {
                                                          return false;
                                                      }

                                                      return xInt >= 2010 && xInt <= 2020;
                                                  });

            var eyrValidator = new FieldValidator("eyr",
                                                  x =>
                                                  {
                                                      if (!int.TryParse(x,
                                                                        out var xInt))
                                                      {
                                                          return false;
                                                      }

                                                      return xInt >= 2020 && xInt <= 2030;
                                                  });

            var hgtValidator = new FieldValidator("hgt",
                                                  x =>
                                                  {
                                                      var cmIndex = x.IndexOf("cm");
                                                      var isCm = cmIndex > 0;

                                                      var inIndex = x.IndexOf("in");
                                                      var isIn = inIndex > 0;

                                                      if (!isCm
                                                          && !isIn)
                                                      {
                                                          return false;
                                                      }

                                                      var value = x.Substring(0,
                                                                              x.Length - 2);

                                                      if (!int.TryParse(value,
                                                                        out var valueInt))
                                                      {
                                                          return false;
                                                      }

                                                      if (isCm)
                                                      {
                                                          return valueInt >= 150 && valueInt <= 193;
                                                      }

                                                      return valueInt >= 59 && valueInt <= 76;
                                                  });

            var hclValidator = new FieldValidator("hcl",
                                                  x => Regex.Match(x,
                                                                   @"^#[0-9a-f]{6}$")
                                                            .Success);

            var eclValues = new HashSet<string>
                            {
                                "amb",
                                "blu",
                                "brn",
                                "gry",
                                "grn",
                                "hzl",
                                "oth"
                            };

            var eclValidator = new FieldValidator("ecl",
                                                  x => eclValues.Contains(x));

            var pidValidator = new FieldValidator("pid",
                                                  x => x.Length == 9
                                                       && int.TryParse(x,
                                                                       out _));

            var partTwoValidator = new PassportValidator(new List<FieldValidator>
                                                         {
                                                             byrValidator,
                                                             iyrValidator,
                                                             eyrValidator,
                                                             hgtValidator,
                                                             hclValidator,
                                                             eclValidator,
                                                             pidValidator
                                                         });

            var partTwoValidPassports = passportModels.Count(partTwoValidator.IsPassportValid);

            Console.WriteLine($"Part two valid passports: {partTwoValidPassports}.");

            Console.ReadKey();
        }

        #endregion
    }
}