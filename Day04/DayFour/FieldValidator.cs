using System;

namespace DayFour
{
    public class FieldValidator
    {
        #region Fields

        private readonly Func<string, bool> fieldValidationMethod;

        #endregion

        #region Constructors

        public FieldValidator(string key,
                              Func<string, bool> fieldValidation)
        {
            Key = key;
            fieldValidationMethod = fieldValidation;
        }

        #endregion

        #region Instance Properties

        public string Key { get; }

        #endregion

        #region Instance Methods

        public bool TestInput(string value)
        {
            return fieldValidationMethod(value);
        }

        #endregion
    }
}