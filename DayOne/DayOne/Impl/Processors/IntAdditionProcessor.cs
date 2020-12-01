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

        protected override void ValidateOperation<T>(MathematicOperation<T> mathematicOperation)
        {
            if (typeof(T) != typeof(int))
            {
                throw new ArgumentException("Processor expects int operands.");
            }

            base.ValidateOperation(mathematicOperation);
        }

        protected override T ProcessInternal<T>(MathematicOperation<T> mathematicOperation)
        {
            var opOne = (int)(object)mathematicOperation.OperandOne;

            var opTwo = (int)(object)mathematicOperation.OperandTwo;

            return (T)Convert.ChangeType(opOne + opTwo,
                                         typeof(T));
        }

        #endregion
    }
}