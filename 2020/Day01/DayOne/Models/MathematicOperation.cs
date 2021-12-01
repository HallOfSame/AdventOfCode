using System.Collections.Generic;

using DayOne.Enums;

namespace DayOne.Models
{
    public class MathematicOperation<T>
    {
        #region Constructors

        public MathematicOperation(List<T> ops,
                                   MathematicOperationType opType)
        {
            Operands = ops;
            OperationType = opType;
        }

        #endregion

        #region Instance Properties

        public List<T> Operands { get; }

        public MathematicOperationType OperationType { get; }

        #endregion
    }
}