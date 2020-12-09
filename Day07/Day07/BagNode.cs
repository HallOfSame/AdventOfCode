using System.Collections.Generic;

namespace Day07
{
    public class BagNode
    {
        #region Constructors

        public BagNode(string color)
        {
            Color = color;
            ChildNodes = new List<NodeLink>();
            ParentLinks = new List<NodeLink>();
        }

        #endregion

        #region Instance Properties

        public List<NodeLink> ChildNodes { get; set; }

        public string Color { get; }

        public List<NodeLink> ParentLinks { get; set; }

        #endregion
    }
}