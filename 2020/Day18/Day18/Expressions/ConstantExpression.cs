namespace Day18.Expressions
{
    public class ConstantExpression : Expression
    {
        #region Constructors

        public ConstantExpression(long value)
        {
            Value = value;
        }

        #endregion

        #region Instance Properties

        public long Value { get; }

        #endregion

        #region Instance Methods

        public override string Display()
        {
            return Value.ToString();
        }

        public override long Evaluate()
        {
            return Value;
        }

        #endregion
    }
}