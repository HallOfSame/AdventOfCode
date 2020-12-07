namespace Day07
{
    public class NodeLink
    {
        #region Constructors

        public NodeLink(BagNode linkedNode,
                        int quantity)
        {
            LinkedNode = linkedNode;
            Quantity = quantity;
        }

        #endregion

        #region Instance Properties

        public BagNode LinkedNode { get; }

        public int Quantity { get; }

        #endregion
    }
}