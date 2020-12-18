namespace Day18.Expressions
{
    public class AdditionExpression : Expression
    {
        #region Constructors

        public AdditionExpression(Expression left,
                                  Expression right)
        {
            Left = left;
            Right = right;
        }

        #endregion

        #region Instance Properties

        public Expression Left { get; }

        public Expression Right { get; }

        #endregion

        #region Instance Methods

        public override string Display()
        {
            return $"{Left.Display()} + {Right.Display()}";
        }

        public override long Evaluate()
        {
            return Left.Evaluate() + Right.Evaluate();
        }

        #endregion
    }
}