using System;

using DayOne.Interfaces;
using DayOne.Models;

namespace DayOne.Impl
{
    public class MathematicOperationEvaluator : IMathematicOperationEvaluator
    {
        #region Fields

        private readonly IOperationProcessorSelector operationProcessorSelector;

        #endregion

        #region Constructors

        public MathematicOperationEvaluator(IOperationProcessorSelector operationProcessorSelector)
        {
            this.operationProcessorSelector = operationProcessorSelector ?? throw new ArgumentNullException(nameof(operationProcessorSelector));
        }

        #endregion

        #region Instance Methods

        public T EvaluateOperation<T>(MathematicOperation<T> operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            var processor = operationProcessorSelector.GetOperationProcessor(operation);

            return processor.ProcessOperation(operation);
        }

        #endregion
    }
}