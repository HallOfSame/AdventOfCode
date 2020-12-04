using System;
using System.Collections.Generic;
using System.Text;

namespace DayFour
{
    public class FieldValidator
    {
        private readonly Func<bool, string> fieldValidationMethod;

        private readonly string key;

        public FieldValidator(string key, Func<bool, string> fieldValidation)
        {
            this.key = key;
            this.fieldValidationMethod = fieldValidation;
        }
    }
}
