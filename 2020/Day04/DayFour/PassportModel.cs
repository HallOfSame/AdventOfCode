using System.Collections.Generic;

namespace DayFour
{
    public class PassportModel
    {
        #region Constructors

        public PassportModel()
        {
            KeyValues = new Dictionary<string, string>();
        }

        #endregion

        #region Instance Properties

        public Dictionary<string, string> KeyValues { get; }

        #endregion

        #region Instance Methods

        public void AddInfo(string key,
                            string value)
        {
            KeyValues.Add(key,
                          value);
        }

        #endregion
    }
}