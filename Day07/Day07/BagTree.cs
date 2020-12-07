using System.Collections.Generic;
using System.Linq;

namespace Day07
{
    // 'Tree' might not be entirely accurate
    public class BagTree
    {
        #region Fields

        private readonly Dictionary<string, BagNode> allNodes;

        private readonly Dictionary<string, int> bagRequirementCache;

        private readonly Dictionary<string, List<string>> parentColorCache;

        #endregion

        #region Constructors

        public BagTree(IEnumerable<BagNode> nodes)
        {
            allNodes = nodes.ToDictionary(x => x.Color,
                                          x => x);

            parentColorCache = new Dictionary<string, List<string>>();
            bagRequirementCache = new Dictionary<string, int>();
        }

        #endregion

        #region Instance Methods

        public List<string> GetAllowedParentColorsForColor(string color)
        {
            if (parentColorCache.TryGetValue(color,
                                             out var allowedParents))
            {
                return allowedParents;
            }

            var allowedOuterBagColors = new HashSet<string>();

            var startNode = allNodes[color];

            foreach (var parent in startNode.ParentLinks)
            {
                // This node color is always allowed
                var allowedColors = new List<string>
                                    {
                                        parent.LinkedNode.Color
                                    };

                allowedColors.AddRange(GetAllowedParentColorsForColor(parent.LinkedNode.Color));

                allowedOuterBagColors.UnionWith(allowedColors);
            }

            var outerColorList = allowedOuterBagColors.ToList();

            parentColorCache[color] = outerColorList;

            return outerColorList;
        }

        public int GetNumberOfBagsRequiredForColor(string color)
        {
            var startNode = allNodes[color];

            if (bagRequirementCache.TryGetValue(color,
                                                out var requiredBags))
            {
                return requiredBags;
            }

            if (!startNode.ChildNodes.Any())
            {
                // No bags required inside this color
                return 0;
            }

            var numberOfBags = 0;

            foreach (var child in startNode.ChildNodes)
            {
                // First get the number of bags required inside this color bag
                var numberOfBagsForColor = GetNumberOfBagsRequiredForColor(child.LinkedNode.Color);

                // Multiply that by the number of this color we need. Then add the quantity of the child color we have
                var numberOfColoredBagsForThisNode = numberOfBagsForColor * child.Quantity + child.Quantity;

                numberOfBags += numberOfColoredBagsForThisNode;
            }

            bagRequirementCache[color] = numberOfBags;

            return numberOfBags;
        }

        #endregion
    }
}