using DayOne.Models;

namespace DayOne.Interfaces
{
    public interface IMathematicOperationEvaluator
    {
        #region Instance Methods

        /// <summary>
        /// Evaluates the operation <paramref name="operation" /> and returns the result.
        /// </summary>
        /// <typeparam name="T">Type we are operating on.</typeparam>
        /// <param name="operation">Model detailing the operation.</param>
        /// <returns>
        /// The result of evaluating <paramref name="operation" />.
        /// </returns>
        T EvaluateOperation<T>(MathematicOperation<T> operation);

        #endregion
    }
}