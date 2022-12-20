using System.Diagnostics;
using System.Text;

using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day20 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            foreach (var node in nodes.ToList())
            {
                Shuffle(node);
            }

            var nodeZero = nodes.First(x => x.Value == 0);

            var distances = new[]
                            {
                                1000,
                                2000,
                                3000
                            }.Select(x => TranslateDistance(x));

            var result = 0l;

            foreach (var distance in distances)
            {
                var currNode = nodeZero;

                for (var i = 0; i < distance; i++)
                {
                    currNode = currNode.Next;
                }

                result += currNode.Value;
            }

            return result.ToString();
        }

        // Convert circling multiple times in to a smaller list sized move
        private long TranslateDistance(long distanceToMove)
        {
            var listLength = nodes.Count;

            var absDistance = Math.Abs(distanceToMove);

            if (absDistance < listLength)
            {
                return distanceToMove;
            }

            // I am not entirely sure why + 1 is needed here
            // But I know it makes the answer correct with TranslateDistance
            var updatedDistance = absDistance % (listLength - 1L);

            return distanceToMove > 0
                       ? updatedDistance
                       : updatedDistance * -1L;
        }

        private void Shuffle(LinkedListNode node)
        {
            var actualDistanceToMove = TranslateDistance(node.Value);

            if (actualDistanceToMove > 0)
            {
                for (var i = 0; i < actualDistanceToMove; i++)
                {
                    var oldNextNode = node.Next; // 2
                    var oldPrevNode = node.Prev; // 4

                    node.Next = oldNextNode.Next; // 1 -> -3
                    node.Prev = oldNextNode; // 2 <- 1

                    oldNextNode.Next = node; // 2 -> 1
                    oldNextNode.Prev = oldPrevNode; // 4 <- 2

                    oldPrevNode.Next = oldNextNode; // 4 -> 2

                    node.Next.Prev = node; // 1 <- -3
                }
            }
            else
            {
                for (var i = actualDistanceToMove; i < 0; i++)
                {
                    var oldNextNode = node.Next; // -3
                    var oldPrevNode = node.Prev; // 1

                    node.Next = oldPrevNode; // 2 -> 1
                    node.Prev = oldPrevNode.Prev; // 4 <- 2
                    
                    oldPrevNode.Next = oldNextNode; // 1 -> -3
                    oldPrevNode.Prev = node; // 2 <- 1

                    oldNextNode.Prev = oldPrevNode; // 1 <- -3

                    node.Prev.Next = node; // 4 -> 2
                }
            }
        }

        private static void Draw(LinkedListNode startNode)
        {
            var sb = new StringBuilder();

            var c = startNode;

            do
            {
                sb.Append($"{c.Value}, ");
                c = c.Next;
            }
            while (c != startNode);

            Console.WriteLine(sb.ToString());
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var decryptionKey = 811589153L;
            var nodeList = nodeClones.ToList();

            nodeClones.ForEach(x => x.Value *= decryptionKey);

            //Console.WriteLine("Initial arrangement:");
            //Draw(nodeList.First());

            // Shuffle 10 times
            for (var i = 0; i < 10; i++)
            {
                foreach (var node in nodeList)
                {
                    Shuffle(node);
                }

                //Console.WriteLine($"After shuffle iteration: {i + 1}");

                //Draw(nodeList.First());
            }

            var nodeZero = nodeClones.First(x => x.Value == 0);

            var distances = new[]
                            {
                                1000,
                                2000,
                                3000
                            }.Select(x => TranslateDistance(x));

            var result = 0L;

            foreach (var distance in distances)
            {
                var currNode = nodeZero;

                for (var i = 0; i < distance; i++)
                {
                    currNode = currNode.Next;
                }

                result += currNode.Value;
            }

            return result.ToString();
        }

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            nodes = new List<LinkedListNode>();
            nodeClones = new List<LinkedListNode>();

            foreach (var line in lines)
            {
                nodes.Add(new LinkedListNode
                          {
                              Value = int.Parse(line)
                          });

                nodeClones.Add(new LinkedListNode
                          {
                              Value = int.Parse(line)
                          });
            }

            HookUpList(nodes);
            HookUpList(nodeClones);
        }

        private void HookUpList(List<LinkedListNode> nodeList)
        {
            for (var i = 0; i < nodeList.Count; i++)
            {
                var current = nodeList[i];
                var next = nodeList[i + 1 == nodeList.Count
                                        ? 0
                                        : i + 1];
                var prev = nodeList[i - 1 < 0
                                        ? nodeList.Count - 1
                                        : i - 1];
                current.Next = next;
                current.Prev = prev;
            }
        }

        private List<LinkedListNode> nodes;
        private List<LinkedListNode> nodeClones;
    }
}

[DebuggerDisplay("{Prev.Value}, ({Value}), {Next.Value}")]
class LinkedListNode
{
    public LinkedListNode Prev { get; set; }

    public LinkedListNode Next { get; set; }

    public long Value { get; set; }
}