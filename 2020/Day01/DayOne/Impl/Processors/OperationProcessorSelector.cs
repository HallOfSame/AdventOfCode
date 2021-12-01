using System;

using DayOne.Enums;
using DayOne.Interfaces;
using DayOne.Models;

namespace DayOne.Impl.Processors
{
    public class OperationProcessorSelector : IOperationProcessorSelector
    {
        #region Instance Methods

        public IOperationProcessor GetOperationProcessor<T>(MathematicOperation<T> mathematicOperation)
        {
            var operandType = typeof(T);

            if (operandType == typeof(int))
            {
                return GetProcessorForIntOperations(mathematicOperation.OperationType);
            }

            throw new NotImplementedException();
        }

        private IOperationProcessor GetProcessorForIntOperations(MathematicOperationType operationType)
        {
            switch (operationType)
            {
                case MathematicOperationType.Add:
                    return new IntAdditionProcessor();
                case MathematicOperationType.Multiply:
                    return new IntMultProcessor();
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}