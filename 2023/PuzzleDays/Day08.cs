using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.FileReaders;
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
            throw new NotImplementedException();
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
