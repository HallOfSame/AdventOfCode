using System.Collections.Generic;

using DayOne.Models;

namespace DayOne.Interfaces
{
    public interface IOperationFactory
    {
        #region Instance Methods

        MathematicOperation<T> CreateAddOperation<T>(List<T> operands);

        MathematicOperation<T> CreateMultiplicationOperation<T>(List<T> operands);

        #endregion
    }
}