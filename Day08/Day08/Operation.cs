namespace Day08
{
    public enum OperationType
    {
        NOP,

        JMP,

        ACC
    }

    public class Operation
    {
        #region Constructors

        public Operation(OperationType type,
                         int argument)
        {
            OperationType = type;
            Argument = argument;
        }

        #endregion

        #region Instance Properties

        public int Argument { get; }

        public OperationType OperationType { get; }

        #endregion
    }
}