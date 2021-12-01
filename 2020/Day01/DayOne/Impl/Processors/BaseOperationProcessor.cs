using System;

using DayOne.Enums;
using DayOne.Interfaces;
using DayOne.Models;

namespace DayOne.Impl.Processors
{
    /// <summary>
    /// A base class for <see cref="IOperationProcessor" /> implementations.
    /// </summary>
    public abstract class BaseOperationProcessor : IOperationProcessor
    {
        #region Instance Properties

        public abstract MathematicOperationType SupportedOperationType { get; }

        #endregion

        #region Instance Methods

        public T ProcessOperation<T>(MathematicOperation<T> mathematicOperation)
        {
            ValidateOperation(mathematicOperation);

            return ProcessInternal(mathematicOperation);
        }

        protected abstract T ProcessInternal<T>(MathematicOperation<T> mathematicOperation);

        /// <summary>
        /// Validates that <paramref name="mathematicOperation" /> is valid. Base class checks the operation matches. Can be overridden to
        /// provide more checks.
        /// </summary>
        /// <typeparam name="T">The type we are operating on.</typeparam>
        /// <param name="mathematicOperation">The operation to validate.</param>
        protected virtual void ValidateOperation<T>(MathematicOperation<T> mathematicOperation)
        {
            if (mathematicOperation.OperationType != SupportedOperationType)
            {
                throw new ArgumentException($"Operation type {mathematicOperation.OperationType} does not match supported type {SupportedOperationType}.");
            }
        }

        #endregion
    }
}