using DayOne.Enums;

namespace DayOne.Models
{
    public class MathematicOperation<T>
    {
        #region Constructors

        public MathematicOperation(T opOne,
                                   T opTwo,
                                   MathematicOperationType opType)
        {
            OperandOne = opOne;
            OperandTwo = opTwo;
            OperationType = opType;
        }

        #endregion

        #region Instance Properties

        public T OperandOne { get; }

        public T OperandTwo { get; }

        public MathematicOperationType OperationType { get; }

        #endregion
    }
}