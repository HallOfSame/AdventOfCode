using DayOne.Enums;
using DayOne.Models;

namespace DayOne.Interfaces
{
    /// <summary>
    /// Interface for classes which process a single type of <see cref="MathematicOperationType" />.
    /// </summary>
    public interface IOperationProcessor
    {
        #region Instance Properties

        /// <summary>
        /// The <see cref="MathematicOperationType" /> this processor can process.
        /// </summary>
        MathematicOperationType SupportedOperationType { get; }

        #endregion

        #region Instance Methods

        /// <summary>
        /// Processes the <paramref name="mathematicOperation" />.
        /// </summary>
        /// <typeparam name="T">The type we are operating on.</typeparam>
        /// <param name="mathematicOperation">The operation to process.</param>
        /// <returns>The result of the operation.</returns>
        T ProcessOperation<T>(MathematicOperation<T> mathematicOperation);

        #endregion
    }
}