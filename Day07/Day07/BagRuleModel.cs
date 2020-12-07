using System.Collections.Generic;

namespace Day07
{
    public class BagRuleModel
    {
        #region Constructors

        public BagRuleModel(string color,
                            List<string> allowedContents)
        {
            BagColor = color;
            AllowedContents = allowedContents;
        }

        #endregion

        #region Instance Properties

        public List<string> AllowedContents { get; }

        public List<string> AllowedIn { get; set; }

        public string BagColor { get; }

        #endregion
    }
}