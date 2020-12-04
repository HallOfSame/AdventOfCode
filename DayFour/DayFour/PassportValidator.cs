using System.Collections.Generic;

namespace DayFour
{
    public class PassportValidator
    {
        #region Fields

        private readonly HashSet<string> requiredKeys;

        #endregion

        #region Constructors

        public PassportValidator(List<string> requiredKeys)
        {
            this.requiredKeys = new HashSet<string>(requiredKeys);
        }

        #endregion

        #region Instance Methods

        public bool IsPassportValid(PassportModel passportModel)
        {
            var passportKeys = new HashSet<string>(passportModel.KeyValues.Keys);

            // If the keys of the passport is a super set, then it has all the required keys
            var valid = passportKeys.IsSupersetOf(requiredKeys);

            return valid;
        }

        #endregion
    }
}