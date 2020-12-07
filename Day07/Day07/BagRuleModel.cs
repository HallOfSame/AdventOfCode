using System.Collections.Generic;

namespace Day07
{
    public class BagRuleModel
    {
        #region Constructors

        public BagRuleModel(string color,
                            List<(string, int)> allowedContents)
        {
            BagColor = color;
            AllowedContents = allowedContents;
        }

        #endregion

        #region Instance Properties

        public List<(string color, int qty)> AllowedContents { get; }

        public string BagColor { get; }

        #endregion
    }
}