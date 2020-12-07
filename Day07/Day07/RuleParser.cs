using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Day07
{
    public class RuleParser
    {
        #region Instance Methods

        public async Task<List<BagRuleModel>> GetBagRulesFromInput(StreamReader streamReader)
        {
            var models = new List<BagRuleModel>();

            while (!streamReader.EndOfStream)
            {
                var nextLine = await streamReader.ReadLineAsync();

                models.Add(ParseLine(nextLine));
            }

            return models;
        }

        private BagRuleModel ParseLine(string line)
        {
            var splitLine = line.Split(" contain ");

            string SanitizeColor(string color)
            {
                string fixedColor;

                var indexOfBags = color.LastIndexOf(" bags");

                if (indexOfBags > 0)
                {
                    fixedColor = color.Substring(0,
                                                 indexOfBags);
                }
                else
                {
                    var indexOfBag = color.LastIndexOf(" bag");

                    if (indexOfBag > 0)
                    {
                        fixedColor = color.Substring(0,
                                                     indexOfBag);
                    }
                    else
                    {
                        fixedColor = color;
                    }
                }

                return fixedColor;
            }

            var ruleColor = SanitizeColor(splitLine[0]);

            var allowedContentSplit = splitLine[1]
                                      .TrimEnd('.')
                                      .Split(',');

            var allowedContentColors = new List<string>();

            foreach (var contentSplit in allowedContentSplit)
            {
                var content = contentSplit.Split(' ',
                                                 2);

                var quantity = content[0];

                var color = SanitizeColor(content[1]);

                allowedContentColors.Add(color);
            }

            return new BagRuleModel(ruleColor,
                                    allowedContentColors);
        }

        #endregion
    }
}