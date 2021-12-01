using DayOne.Models;

namespace DayOne.Interfaces
{
    public interface IOperationProcessorSelector
    {
        #region Instance Methods

        /// <summary>
        /// Gets the proper <see cref="IOperationProcessor" /> to handle <paramref name="mathematicOperation" />.
        /// </summary>
        /// <typeparam name="T">The type we are operating on.</typeparam>
        /// <param name="mathematicOperation">The operation.</param>
        /// <returns>
        /// A <see cref="IOperationProcessor" /> to handle <paramref name="mathematicOperation" />.
        /// </returns>
        IOperationProcessor GetOperationProcessor<T>(MathematicOperation<T> mathematicOperation);

        #endregion
    }
}