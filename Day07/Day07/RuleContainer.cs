using System.Collections.Generic;
using System.Linq;

namespace Day07
{
    public class RuleContainer
    {
        #region Fields

        private Dictionary<string, BagRuleModel> ruleDictionary;

        #endregion

        #region Constructors

        public RuleContainer(IEnumerable<BagRuleModel> models)
        {
            InitializeRules(models);
        }

        #endregion

        #region Instance Methods

        public List<string> GetOuterColorsAllowingBag(string bagColor,
                                                      bool recursed = false)
        {
            var outerList = new List<string>();

            foreach (var allowedInColor in ruleDictionary[bagColor]
                .AllowedIn)
            {
                var outerBagAllowance = ruleDictionary[allowedInColor]
                    .AllowedIn;

                if (outerBagAllowance.Any())
                {
                    outerList.AddRange(outerBagAllowance.SelectMany(x => GetOuterColorsAllowingBag(x, true)));
                }
                else
                {
                    outerList.Add(allowedInColor);
                }
            }

            if (recursed)
            {
                outerList.Add(bagColor);
            }

            return outerList.Distinct()
                            .ToList();
        }

        private void InitializeRules(IEnumerable<BagRuleModel> rules)
        {
            ruleDictionary = rules.ToDictionary(x => x.BagColor,
                                                x => x);

            foreach (var rule in ruleDictionary.Values)
            {
                var allowedInListForRule = ruleDictionary.Values.Where(x => x.AllowedContents.Contains(rule.BagColor));

                rule.AllowedIn = allowedInListForRule.Select(x => x.BagColor)
                                                     .ToList();
            }
        }

        #endregion
    }
}