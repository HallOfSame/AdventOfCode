using System;

using DayOne.Enums;
using DayOne.Models;

namespace DayOne.Impl.Processors
{
    public class IntMultProcessor : BaseOperationProcessor
    {
        #region Instance Properties

        public override MathematicOperationType SupportedOperationType
        {
            get
            {
                return MathematicOperationType.Multiply;
            }
        }

        #endregion

        #region Instance Methods

        protected override T ProcessInternal<T>(MathematicOperation<T> mathematicOperation)
        {
            var multResult = 1;

            foreach (var op in mathematicOperation.Operands)
                multResult *= (int)(object)op;

            return (T)Convert.ChangeType(multResult,
                                         typeof(T));
        }

        protected override void ValidateOperation<T>(MathematicOperation<T> mathematicOperation)
        {
            if (typeof(T) != typeof(int))
            {
                throw new ArgumentException("Processor expects int operands.");
            }

            base.ValidateOperation(mathematicOperation);
        }

        #endregion
    }
}