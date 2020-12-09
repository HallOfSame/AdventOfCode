using System;

using DayOne.Enums;
using DayOne.Models;

namespace DayOne.Impl.Processors
{
    public class IntAdditionProcessor : BaseOperationProcessor
    {
        #region Instance Properties

        public override MathematicOperationType SupportedOperationType
        {
            get
            {
                return MathematicOperationType.Add;
            }
        }

        #endregion

        #region Instance Methods

        protected override T ProcessInternal<T>(MathematicOperation<T> mathematicOperation)
        {
            var sum = 0;

            foreach (var op in mathematicOperation.Operands)
                sum += (int)(object)op;

            return (T)Convert.ChangeType(sum,
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