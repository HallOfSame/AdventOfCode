namespace IntCodeInterpreter.Models
{
    public class Parameter
    {
        #region Constructors

        public Parameter(int value)
        {
            Value = value;
        }

        #endregion

        #region Instance Properties

        public int Value { get; }

        #endregion
    }
}