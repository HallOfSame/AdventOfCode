using System.Collections.Generic;

using DayOne.Enums;
using DayOne.Interfaces;
using DayOne.Models;

namespace DayOne.Impl
{
    public class OperationFactory : IOperationFactory
    {
        #region Instance Methods

        public MathematicOperation<T> CreateAddOperation<T>(List<T> operands)
        {
            return new MathematicOperation<T>(operands,
                                              MathematicOperationType.Add);
        }

        public MathematicOperation<T> CreateMultiplicationOperation<T>(List<T> operands)
        {
            return new MathematicOperation<T>(operands,
                                              MathematicOperationType.Multiply);
        }

        #endregion
    }
}