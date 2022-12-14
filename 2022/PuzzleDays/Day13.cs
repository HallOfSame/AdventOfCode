using System.Diagnostics;

using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day13 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var validator = new PacketValidator();

            var result = packetPairs.Select((pair,
                                             idx) => new
                                                     {
                                                         Index = idx + 1,
                                                         Valid = validator.ArePacketsInOrder(pair.left,
                                                                                             pair.right)
                                                     })
                                    .Where(x => x.Valid)
                                    .Select(x => x.Index)
                                    .Sum();

            return result.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var allPackets = packetPairs.SelectMany(x => new[]
                                                         {
                                                             x.left,
                                                             x.right
                                                         })
                                        .ToList();

            var dividerOneStr = "[[2]]";
            var dividerTwoStr = "[[6]]";

            var parser = new PacketParser();

            var dividerOne = new Packet
                             {
                                 Data = parser.ParsePacketData(dividerOneStr),
                                 OriginalString = dividerOneStr
                             };

            var dividerTwo = new Packet
                             {
                                 Data = parser.ParsePacketData(dividerTwoStr),
                                 OriginalString = dividerTwoStr
                             };

            allPackets.Add(dividerOne);
            allPackets.Add(dividerTwo);

            var orderedPackets = allPackets.OrderBy(x => x,
                                                    new PacketComparer())
                                           .ToList();

            return ((orderedPackets.IndexOf(dividerOne) + 1) * (orderedPackets.IndexOf(dividerTwo) + 1)).ToString();
        }

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            var parser = new PacketParser();

            packetPairs = new List<(Packet, Packet)>();

            foreach (var packetPair in lines.Chunk(3))
            {
                var firstPacket = new Packet
                                  {
                                      OriginalString = packetPair[0],
                                      Data = parser.ParsePacketData(packetPair[0])
                                  };

                var secondPacket = new Packet
                                   {
                                       OriginalString = packetPair[1],
                                       Data = parser.ParsePacketData(packetPair[1])
                                   };

                packetPairs.Add((firstPacket, secondPacket));
            }
        }

        private List<(Packet left, Packet right)> packetPairs;
    }
}

class PacketComparer : IComparer<Packet>
{
    private readonly PacketValidator validator = new();

    public int Compare(Packet x,
                       Packet y)
    {
        return validator.CompareData(x.Data,
                                     y.Data);
    }
}

class PacketValidator
{
    public bool ArePacketsInOrder(Packet left,
                                  Packet right)
    {
        return CompareData(left.Data,
                           right.Data)
               <= 0;
    }

    public int CompareData(PacketData leftData,
                            PacketData rightData)
    {
        if (leftData is IntegerData lInt
            && rightData is IntegerData rInt)
        {
            // Math.Sign just converts to 1, 0, -1
            return Math.Sign(lInt.Value - rInt.Value);
        }

        // Fix up left or right if one is a list and the other is not
        if (leftData is ListData
            && rightData is IntegerData)
        {
            rightData = new ListData
                        {
                            List = new List<PacketData>
                                   {
                                       rightData
                                   }
                        };
        }
        else if (leftData is IntegerData
                 && rightData is ListData)
        {
            leftData = new ListData
                        {
                            List = new List<PacketData>
                                   {
                                       leftData
                                   }
                        };
        }

        return CompareLists((ListData)leftData,
                            (ListData)rightData);
    }

    private int CompareLists(ListData leftList,
                             ListData rightList)
    {
        var maxIndexToCheck = Math.Min(leftList.List.Count,
                                       rightList.List.Count);

        // Iterate the left list
        for (var i = 0; i < maxIndexToCheck; i++)
        {
            var left = leftList.List[i];

            var right = rightList.List[i];

            // Compare the item at index i in both lists
            var currentItemValidation = CompareData(left,
                                                    right);

            if (currentItemValidation != 0)
            {
                return currentItemValidation;
            }
        }

        return Math.Sign(leftList.List.Count - rightList.List.Count);
    }
}

[DebuggerDisplay("Integer: {Value}")]
class IntegerData : PacketData
{
    public int Value { get; set; }
}

[DebuggerDisplay("List: Len {List.Count}")]
class ListData : PacketData
{
    public List<PacketData> List { get; set; }
}

class PacketData
{

}

[DebuggerDisplay("{OriginalString}")]
class Packet
{
    public string OriginalString { get; set; }

    public PacketData Data { get; set; }
}

class PacketParser
{
    public PacketData ParsePacketData(string packet)
    {
        PacketData rootData;

        var firstChar = packet[0];

        var index = 0;

        if (firstChar == '[')
        {
            // Start a list
            rootData = ParseList(packet,
                                 ref index);
        }
        else
        {
            rootData = ParseInt(packet,
                                ref index);
        }

        if (rootData is null)
        {
            throw new Exception($"Failed to parse {packet}");
        }

        return rootData;
    }

    private ListData ParseList(string packet,
                               ref int startingChar)
    {
        var newList = new ListData
                      {
                          List = new List<PacketData>()
                      };

        for (var i = startingChar + 1; i < packet.Length; i++)
        {
            var currentChar = packet[i];

            if (currentChar == '[')
            {
                newList.List.Add(ParseList(packet, ref i));
            }
            else if (currentChar == ']')
            {
                // List is done
                startingChar = i;
                return newList;
            }
            else if (currentChar is >= '0' and <= '9')
            {
                newList.List.Add(ParseInt(packet,
                                          ref i));
            }
        }

        throw new Exception("Got to end of un-closed list");
    }

    private IntegerData ParseInt(string packet,
                                 ref int index)
    {
        var stopIndex = index;

        for (var i = index + 1; i < packet.Length; i++)
        {
            var currentChar = packet[i];

            if (currentChar is ',' or ']')
            {
                break;
            }

            stopIndex++;
        }

        var newData = new IntegerData
                      {
                          Value = int.Parse(packet.Substring(index,
                                                             stopIndex - index + 1))
                      };

        index = stopIndex;

        return newData;
    }
}