using System.Collections.Generic;

namespace DayFour
{
    public class PassportValidator
    {
        #region Fields

        private readonly List<FieldValidator> validators;

        #endregion

        #region Constructors

        public PassportValidator(List<FieldValidator> validators)
        {
            this.validators = validators;
        }

        #endregion

        #region Instance Methods

        public bool IsPassportValid(PassportModel passportModel)
        {
            foreach (var validator in validators)
            {
                if (!passportModel.KeyValues.TryGetValue(validator.Key,
                                                         out var value))
                {
                    return false;
                }

                if (!validator.TestInput(value))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}