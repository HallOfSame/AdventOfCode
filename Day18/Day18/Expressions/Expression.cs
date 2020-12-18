using System;
using System.Collections.Generic;
using System.Text;

namespace Day18.Expressions
{
    public abstract class Expression
    {
        #region Instance Methods

        public abstract string Display();

        public abstract long Evaluate();

        #endregion
    }
}
