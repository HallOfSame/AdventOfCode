using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.FileReaders;
using Helpers.MathAndSuch;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day08 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var currentNode = nodeMap["AAA"];

            var stepsTaken = 0;

            foreach (var direction in NextDirection())
            {
                currentNode = direction == Direction.Right ? currentNode.Right : currentNode.Left;

                stepsTaken++;

                if (currentNode.Label == "ZZZ")
                {
                    break;
                }
            }

            return stepsTaken.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            // Some of this took time & looking at the data to figure out
            var startingNodes = nodeMap.Values.Where(x => x.Label.EndsWith("A"))
                .ToArray();

            var loopData = new List<(int stepsToReachGoal, int loopSize)>();

            // This whole foreach loop I wrote up front
            foreach (var startingNode in startingNodes)
            {
                var stepsTakenForThisNodeToLoop = 0;
                var reachedGoalForThisNode = false;
                var stepsToReachGoal = 0;
                var currentNode = startingNode;

                // For every node we start at figure out:
                // How long it takes to reach the goal
                // How long after reaching the goal it takes us to loop back to it
                // This is all pretty fast, it's just like running part 1 a few extra times
                foreach (var direction in NextDirection())
                {
                    currentNode = direction == Direction.Right ? currentNode.Right : currentNode.Left;

                    stepsTakenForThisNodeToLoop++;

                    if (currentNode.Label.EndsWith("Z"))
                    {
                        if (!reachedGoalForThisNode)
                        {
                            reachedGoalForThisNode = true;
                            stepsToReachGoal = stepsTakenForThisNodeToLoop;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                loopData.Add((stepsToReachGoal, stepsTakenForThisNodeToLoop - stepsToReachGoal));
            }

            // After that I got stuck for a while
            // Originally the loop above was checking how long it took us to return to a node we've seen and not actually to reach the goal
            // So I thought there would be some kind of offset needed between the loops and that is was some chinese remainder theorem junk or something
            // Then I realized we need to end up at the goal, so the loop size above should be steps to go from goal and get back to it

            // Turns out, for all the starting points in my input, it's just one big loop from the start node to the end node
            // Which means no offset (yay) so just get the LCM between the loop sizes and we're done
            var answer = MathFunctions.LeastCommonMultiple(loopData.Select(x => (long)x.loopSize));
            
            return answer.ToString();
        }

        private IEnumerable<Direction> NextDirection()
        {
            var currentIndex = 0;

            while (true)
            {
                if (currentIndex >= directions.Count)
                {
                    currentIndex = 0;
                }

                yield return directions[currentIndex];

                currentIndex++;
            }
        }

        public override async Task ReadInput()
        {
            var strings = await new StringFileReader().ReadInputFromFile();

            directions = strings[0]
                .ToArray()
                .Select(x => x == 'L' ? Direction.Left : Direction.Right)
                .ToList();
            
            nodeMap = new Dictionary<string, Node>();

            for (var i = 2; i < strings.Count; i++)
            {
                var split = strings[i]
                    .Split(" = ");

                var nodeLabel = split[0];

                var lrNodes = split[1]
                    [1..^1]
                    .Split(", ");

                var leftNode = lrNodes[0];
                var rightNode = lrNodes[1];

                if (!nodeMap.TryGetValue(leftNode, out var leftNodeObj))
                {
                    leftNodeObj = new Node
                    {
                        Label = leftNode,
                    };
                    nodeMap[leftNode] = leftNodeObj;
                }

                if (!nodeMap.TryGetValue(rightNode, out var rightNodeObj))
                {
                    rightNodeObj = new Node
                    {
                        Label = rightNode,
                    };
                    nodeMap[rightNode] = rightNodeObj;
                }

                if (!nodeMap.TryGetValue(nodeLabel, out var existingCurrentNodeObj))
                {
                    existingCurrentNodeObj = new Node
                    {
                        Label = nodeLabel,
                        Left = leftNodeObj,
                        Right = rightNodeObj,
                    };
                    nodeMap[nodeLabel] = existingCurrentNodeObj;
                }
                else
                {
                    existingCurrentNodeObj.Left = leftNodeObj;
                    existingCurrentNodeObj.Right = rightNodeObj;
                }
            }
        }

        private List<Direction> directions;
        private Dictionary<string, Node> nodeMap;

        [DebuggerDisplay("{Label} ({Left.Label}, {Right.Label})")]
        class Node
        {
            public string Label { get; set; }

            public Node Left { get; set; }

            public Node Right { get; set; }
        }

        enum Direction
        {
            Left,
            Right,
        }
    }
}
