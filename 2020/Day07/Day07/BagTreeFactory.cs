using System.Collections.Generic;
using System.Linq;

namespace Day07
{
    public class BagTreeFactory
    {
        #region Instance Methods

        public BagTree BuildTree(IList<BagRuleModel> ruleModels)
        {
            // Build tree with nodes
            var modelDictionary = ruleModels.ToDictionary(x => x.BagColor,
                                                          x => x);

            // Top level will be bag colors where no other bag can contain them
            var topLevelBags = ruleModels.Where(top => !ruleModels.Any(mod => mod.AllowedContents.Any(content => content.color == top.BagColor)));

            var allNodes = new List<BagNode>();

            foreach (var topLevel in topLevelBags)
            {
                allNodes.Add(BuildNodeFromModel(topLevel,
                                                allNodes,
                                                modelDictionary));
            }

            return new BagTree(allNodes);
        }

        private BagNode BuildNodeFromModel(BagRuleModel bagRuleModel,
                                           IList<BagNode> allNodes,
                                           IDictionary<string, BagRuleModel> modelDictionary)
        {
            var node = new BagNode(bagRuleModel.BagColor);

            foreach (var (color, qty) in bagRuleModel.AllowedContents)
            {
                var childNode = GetNodeForColor(color,
                                                allNodes,
                                                modelDictionary);

                node.ChildNodes.Add(new NodeLink(childNode,
                                                 qty));

                childNode.ParentLinks.Add(new NodeLink(node,
                                                       1));
            }

            return node;
        }

        private BagNode GetNodeForColor(string color,
                                        IList<BagNode> allNodes,
                                        IDictionary<string, BagRuleModel> modelDictionary)
        {
            var existingNode = allNodes.FirstOrDefault(x => x.Color == color);

            if (existingNode != null)
            {
                return existingNode;
            }

            var newNode = BuildNodeFromModel(modelDictionary[color],
                                             allNodes,
                                             modelDictionary);
            allNodes.Add(newNode);
            return newNode;
        }

        #endregion
    }
}