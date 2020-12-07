using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Day07
{
    public class Program
    {
        #region Class Methods

        public static async Task Main(string[] args)
        {
            List<BagRuleModel> models;

            await using (var file = File.OpenRead("PuzzleInput.txt"))
            {
                using var streamReader = new StreamReader(file);

                models = await new RuleParser().GetBagRulesFromInput(streamReader);
            }

            // Build tree with nodes
            var modelDictionary = models.ToDictionary(x => x.BagColor,
                                                      x => x);

            // Top level will be bag colors where no other bag can contain them
            var topLevelBags = models.Where(top => !models.Any(mod => mod.AllowedContents.Any(content => content.color == top.BagColor)));

            var allNodes = new List<BagNode>();

            BagNode GetNodeForColor(string color)
            {
                var existingNode = allNodes.FirstOrDefault(x => x.Color == color);

                if (existingNode != null)
                {
                    return existingNode;
                }

                var newNode = BuildNodeFromModel(modelDictionary[color]);
                allNodes.Add(newNode);
                return newNode;
            }

            BagNode BuildNodeFromModel(BagRuleModel bagRuleModel)
            {
                var node = new BagNode(bagRuleModel.BagColor);

                foreach(var child in bagRuleModel.AllowedContents)
                {
                    var childNode = GetNodeForColor(child.color);

                    node.ChildNodes.Add(new NodeLink(childNode,
                                                     child.qty));

                    childNode.ParentLinks.Add(new NodeLink(node,
                                                           1));
                }

                return node;
            }

            var topLevelNodes = new List<BagNode>();

            foreach (var topLevel in topLevelBags)
            {
                var node = BuildNodeFromModel(topLevel);

                topLevelNodes.Add(node);
            }

            var desiredColor = "shiny gold";

            var desiredNode = GetNodeForColor(desiredColor);


            // Part one
            var allowedOuterBagColors = new HashSet<string>();

            List<string> GetAllowedParentsForNode(BagNode node)
            {
                // This node color is always allowed
                var allowedColors = new List<string>
                                    {
                                        node.Color
                                    };

                foreach (var parent in node.ParentLinks)
                {
                    allowedColors.AddRange(GetAllowedParentsForNode(parent.LinkedNode));
                }

                return allowedColors;
            }

            foreach (var parent in desiredNode.ParentLinks)
            { 
                allowedOuterBagColors.UnionWith(GetAllowedParentsForNode(parent.LinkedNode));
            }
            
            var outerBags = allowedOuterBagColors.Count;

            Console.WriteLine($"{desiredColor} can be in {outerBags} different bags.");

            // Part two
            int GetNumberOfBagsRequiredForNode(BagNode startNode)
            {
                var numberOfBags = 0;

                if (!startNode.ChildNodes.Any())
                {
                    // No bags required inside the node
                    return 0;
                }

                foreach(var child in startNode.ChildNodes)
                {
                    // First get the number of bags required by a single child of that color
                    var numberOfBagsForColor = GetNumberOfBagsRequiredForNode(child.LinkedNode);

                    // Then multiply by size of the link
                    // And add on the actual bags holding all those bags
                    var numberOfColoredBagsForThisNode = (numberOfBagsForColor * child.Quantity) + child.Quantity;

                    numberOfBags += numberOfColoredBagsForThisNode;
                }

                return numberOfBags;
            }

            var totalBagsInShinyGold = GetNumberOfBagsRequiredForNode(desiredNode);

            Console.WriteLine($"A {desiredColor} bag requires {totalBagsInShinyGold} bags inside of it.");
        }

        #endregion
    }
}