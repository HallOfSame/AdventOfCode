using DayOne.Models;

namespace DayOne.Interfaces
{
    public interface ISumOperationFactory
    {
        #region Instance Methods

        /// <summary>
        /// Returns a <see cref="MathematicOperation{T}" /> to represent the sum of <paramref name="valueOne" /> and
        /// <paramref name="valueTwo" />.
        /// </summary>
        /// <typeparam name="T">The type we are operating on.</typeparam>
        /// <param name="valueOne">The first value.</param>
        /// <param name="valueTwo">The second value.</param>
        /// <returns>
        /// An operation representing the sum of the values.
        /// </returns>
        MathematicOperation<T> GetSumOperation<T>(T valueOne,
                                                  T valueTwo);

        #endregion
    }
}