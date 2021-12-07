namespace IntCodeInterpreter.Models
{
    public class Parameter
    {
        #region Constructors

        public Parameter(long value,
                         ParameterMode mode)
        {
            Value = value;
            Mode = mode;
        }

        #endregion

        #region Instance Properties

        public ParameterMode Mode { get; }

        public long Value { get; }

        #endregion
    }

    public enum ParameterMode
    {
        Position = 0,

        Immediate = 1,

        Relative = 2
    }
}