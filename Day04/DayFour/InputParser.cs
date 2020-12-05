using System.Collections.Generic;
using System.IO;

namespace DayFour
{
    public class InputParser
    {
        #region Instance Methods

        public List<PassportModel> ParseFile(StreamReader fileStream)
        {
            var passportLines = ParseLinesFromFile(fileStream: fileStream);

            var passports = GetPassportModelsFromLineData(passportLines: passportLines);

            return passports;
        }

        private List<PassportModel> GetPassportModelsFromLineData(List<List<string>> passportLines)
        {
            var passports = new List<PassportModel>();

            foreach (var passport in passportLines)
            {
                var passportModel = new PassportModel();

                foreach (var line in passport)
                {
                    var pairs = line.Split(' ');

                    foreach (var pair in pairs)
                    {
                        var splitPair = pair.Split(':');

                        passportModel.AddInfo(splitPair[0],
                                              splitPair[1]);
                    }
                }

                passports.Add(passportModel);
            }

            return passports;
        }

        #endregion

        #region Class Methods

        private static List<List<string>> ParseLinesFromFile(StreamReader fileStream)
        {
            var passportLines = new List<List<string>>();

            var currentPassport = new List<string>();

            while (!fileStream.EndOfStream)
            {
                var line = fileStream.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    passportLines.Add(currentPassport);
                    currentPassport = new List<string>();
                    continue;
                }

                currentPassport.Add(line);
            }

            // Since we end the while loop before adding this last one
            passportLines.Add(currentPassport);

            return passportLines;
        }

        #endregion
    }
}