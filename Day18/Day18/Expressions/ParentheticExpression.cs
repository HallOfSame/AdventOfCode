namespace Day18.Expressions
{
    public class ParentheticExpression : Expression
    {
        #region Constructors

        public ParentheticExpression(Expression inner)
        {
            Inner = inner;
        }

        #endregion

        #region Instance Properties

        public Expression Inner { get; }

        #endregion

        #region Instance Methods

        public override string Display()
        {
            return $"({Inner.Display()})";
        }

        public override long Evaluate()
        {
            return Inner.Evaluate();
        }

        #endregion
    }
}