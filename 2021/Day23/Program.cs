using System.Diagnostics;

using Helpers.Extensions;
using Helpers.Maps;
using Helpers.Structure;

using Spectre.Console;

var solver = new Solver(new Day23Problem());

await solver.Solve();

class Day23Problem : ProblemBase
{
    Burrow startingPosition;

    #region Instance Methods

    public override async Task ReadInput()
    {
        startingPosition = new Burrow
                           {
                               Hallway = Enumerable.Repeat(MapItem.Empty,
                                                           11)
                                                   .ToArray(),
                               Rooms = new Room[]
                                       {
                                           new()
                                           {
                                               Index = 2,
                                               Top = MapItem.Desert,
                                               Bottom = MapItem.Bronze
                                           },
                                           new()
                                           {
                                               Index = 4,
                                               Top = MapItem.Desert,
                                               Bottom = MapItem.Amber
                                           },
                                           new()
                                           {
                                               Index = 6,
                                               Top = MapItem.Copper,
                                               Bottom = MapItem.Amber
                                           },
                                           new()
                                           {
                                               Index = 8,
                                               Top = MapItem.Bronze,
                                               Bottom = MapItem.Copper
                                           }
                                       }
                           };
    }

    protected override async Task<string> SolvePartOneInternal()
    {
        var solve = new BurrowSolver();

        var energyNeeded = solve.GetEnergyRequired(startingPosition);

        return energyNeeded.ToString();
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
    }

    #endregion
}

class BurrowSolver
{
    public int GetEnergyRequired(Burrow start)
    {
        // Copying the base alg from day 15
        var goal = new Burrow
                   {
                       Hallway = Enumerable.Repeat(MapItem.Empty,
                                                   11)
                                           .ToArray(),
                       Rooms = new Room[]
                               {
                                   new()
                                   {
                                       Index = 2,
                                       Top = MapItem.Amber,
                                       Bottom = MapItem.Amber
                                   },
                                   new()
                                   {
                                       Index = 4,
                                       Top = MapItem.Bronze,
                                       Bottom = MapItem.Bronze
                                   },
                                   new()
                                   {
                                       Index = 6,
                                       Top = MapItem.Copper,
                                       Bottom = MapItem.Copper
                                   },
                                   new()
                                   {
                                       Index = 8,
                                       Top = MapItem.Desert,
                                       Bottom = MapItem.Desert
                                   }
                               }
                   };

        // This is more or less an implementation of Dijkstra's alg

        // Track in the value, the min energy required to get to key
        var energyRequirement = new Dictionary<Burrow, int>()
                                {
                                    {
                                        start, 0
                                    }
                                };

        var notVisited = new PriorityQueue<Burrow, int>();

        notVisited.Enqueue(start,
                           0);

        while (notVisited.Count != 0)
        {
            var current = notVisited.Dequeue();

            var possibleMoves = current.GetPossibleMoves();

            foreach (var (newBurrow, energyAdded) in possibleMoves)
            {
                var newEnergyRequired = energyRequirement[current] + energyAdded;

                var currentFoundDistance = energyRequirement.GetValueOrDefault(newBurrow,
                                                                               int.MaxValue);

                if (newEnergyRequired < currentFoundDistance)
                {
                    energyRequirement[newBurrow] = newEnergyRequired;
                    notVisited.Enqueue(newBurrow,
                                       newEnergyRequired);
                }
            }
        }

        return energyRequirement[goal];
    }
}

class Burrow
{
    public Room[] Rooms { get; set; }

    public MapItem[] Hallway { get; set; }

    public (Burrow, int)[] GetPossibleMoves()
    {
        // Possible moves include:
        var possibleMoves = new List<(Burrow, int)>();

        // Moving something out of a room:
        foreach (var room in Rooms)
        {
            if (room.IsEmpty())
            {
                continue;
            }

            possibleMoves.AddRange(GetMoveOut(room));

            var possibleRoomToRoomMove = GetMoveInFromOtherRoom(room);

            if (possibleRoomToRoomMove != null)
            {
                possibleMoves.Add(possibleRoomToRoomMove.Value);
            }
        }

        // Moving something in to a room:
        for(var i = 0; i < Hallway.Length; i++)
        {
            var tile = Hallway[i];

            if (tile == MapItem.Empty)
            {
                continue;
            }

            var possibleMove = GetMoveIn(i);

            if (possibleMove != null)
            {
                possibleMoves.Add(possibleMove.Value);
            }
        }

        return possibleMoves.ToArray();
    }

    public (Burrow, int)? GetMoveInFromOtherRoom(Room room)
    {
        var itemToMove = room.Top != MapItem.Empty
                             ? room.Top
                             : room.Bottom;

        var roomIndex = itemToMove switch
        {
            MapItem.Amber => 2,
            MapItem.Bronze => 4,
            MapItem.Copper => 6,
            MapItem.Desert => 8,
            _ => throw new ArgumentOutOfRangeException(nameof(itemToMove))
        };

        if (room.Index == roomIndex)
        {
            // Already in the correct room
            return null;
        }

        var destinationRoom = Rooms[(roomIndex / 2) - 1];

        var lower = room.Index > destinationRoom.Index
                        ? destinationRoom.Index
                        : room.Index;

        var upper = room.Index > destinationRoom.Index
                        ? room.Index
                        : destinationRoom.Index;

        var spaceToMove = Hallway[lower..upper]
            .All(x => x == MapItem.Empty);

        if (!spaceToMove)
        {
            // Something is blocking us from moving
            return null;
        }

        if (!destinationRoom.HasSpace()
            || destinationRoom.Bottom != itemToMove)
        {
            // It's not valid to move this item back in yet
            // The room is full, or has the wrong type still in it
            return null;
        }

        var updatedHallway = (MapItem[])Hallway.Clone();

        var addToBottom = destinationRoom.Bottom == MapItem.Empty;

        var updatedDestinationRoom = new Room
                                     {
                                         Top = addToBottom
                                                   ? destinationRoom.Top
                                                   : itemToMove,
                                         Bottom = addToBottom
                                                      ? itemToMove
                                                      : destinationRoom.Bottom,
                                         Index = destinationRoom.Index
                                     };

        var takeFromTop = room.Top != MapItem.Empty;

        var updatedSourceRoom = new Room
                                {
                                    Top = MapItem.Empty,
                                    Bottom = takeFromTop
                                                 ? room.Bottom
                                                 : MapItem.Empty,
                                    Index = room.Index
                                };

        var moveDistance = Math.Abs(room.Index - destinationRoom.Index);

        moveDistance += addToBottom
                            ? 2
                            : 1;

        moveDistance += takeFromTop
                            ? 1
                            : 2;

        var energyUsed = GetMoveEnergy(moveDistance,
                                       itemToMove);

        return (new Burrow
                {
                    Hallway = updatedHallway,
                    // The classic double ternary
                    Rooms = this.Rooms.Select(r => r.Index == updatedDestinationRoom.Index ? updatedDestinationRoom : r.Index == room.Index ? updatedSourceRoom : r.Clone())
                                .ToArray()
                }, energyUsed);
    }

    public (Burrow, int)? GetMoveIn(int hallwayIndex)
    {
        var itemToMove = Hallway[hallwayIndex];

        var roomIndex = itemToMove switch
        {
            MapItem.Amber => 2,
            MapItem.Bronze => 4,
            MapItem.Copper => 6,
            MapItem.Desert => 8,
            _ => throw new ArgumentOutOfRangeException(nameof(itemToMove))
        };

        var room = Rooms[(roomIndex / 2) - 1];

        var lower = hallwayIndex > roomIndex
                        ? roomIndex
                        : hallwayIndex + 1;

        var upper = hallwayIndex > roomIndex
                        ? hallwayIndex - 1
                        : roomIndex;

        var spaceToMove = Hallway[lower..upper]
            .All(x => x == MapItem.Empty);

        if (!spaceToMove)
        {
            // Something is blocking us from moving
            return null;
        }

        if (room.Top != MapItem.Empty || (room.Bottom != MapItem.Empty && room.Bottom != itemToMove))
        {
            // It's not valid to move this item back in yet
            // The room is full, or has the wrong type still in it
            return null;
        }

        var updatedHallway = (MapItem[])Hallway.Clone();

        updatedHallway[hallwayIndex] = MapItem.Empty;

        var addToBottom = room.Bottom == MapItem.Empty;

        var updatedRoom = new Room
                          {
                              Top = addToBottom
                                        ? room.Top
                                        : itemToMove,
                              Bottom = addToBottom
                                           ? itemToMove
                                           : room.Bottom,
                              Index = room.Index
                          };

        var moveDistance = Math.Abs(roomIndex - hallwayIndex);

        moveDistance += addToBottom
                            ? 2
                            : 1;

        var energyUsed = GetMoveEnergy(moveDistance,
                                       itemToMove);

        return (new Burrow
                {
                    Hallway = updatedHallway,
                    Rooms = this.Rooms.Select(r => r.Index == updatedRoom.Index
                                                       ? updatedRoom
                                                       : r.Clone())
                                .ToArray()
                }, energyUsed);
    }

    public int GetMoveEnergy(int distance,
                             MapItem tile)
    {
        var modifier = tile switch
        {
            MapItem.Amber => 1,
            MapItem.Bronze => 10,
            MapItem.Copper => 100,
            MapItem.Desert => 1000
        };

        return distance * modifier;
    }

    public (Burrow, int)[] GetMoveOut(Room room)
    {
        var openHallwaySpots = new List<int>();

        // Start by checking to the left

        Func<int, int> checkLeft = idx => idx - 1;
        Func<int, int> checkRight = idx => idx + 1;

        void PopulateOpenSpots(Func<int, int> alterIndexFunc)
        {
            var currentCheck = room.Index;

            while(true)
            {
                bool IsValid(int index)
                {
                    // Can't stop on top of a room
                    return index != 2 && index != 4 && index != 6 && index != 8;
                }

                currentCheck = alterIndexFunc(currentCheck);

                if (currentCheck < 0
                    || currentCheck >= Hallway.Length)
                {
                    break;
                }

                if (Hallway[currentCheck] == MapItem.Empty)
                {
                    if (IsValid(currentCheck))
                    {
                        openHallwaySpots.Add(currentCheck);
                    }
                }
                else
                {
                    // Can break since we hit something blocking the way
                    break;
                }
            }
        }

        PopulateOpenSpots(checkLeft);
        PopulateOpenSpots(checkRight);

        var moveTop = room.Top != MapItem.Empty;

        var itemToMove = moveTop
                             ? room.Top
                             : room.Bottom;

        var updatedRoom = new Room
                          {
                              Index = room.Index,
                              Top = moveTop
                                        ? MapItem.Empty
                                        : room.Top,
                              Bottom = moveTop
                                           ? room.Bottom
                                           : MapItem.Empty
                          };

        var possibleNewBurrows = new List<(Burrow, int)>(openHallwaySpots.Count);

        foreach (var index in openHallwaySpots)
        {
            var moveDistance = Math.Abs(room.Index - index);

            moveDistance += moveTop
                                ? 1
                                : 2;

            var moveEnergy = GetMoveEnergy(moveDistance,
                                           itemToMove);

            var updatedHallway = (MapItem[])Hallway.Clone();

            updatedHallway[index] = itemToMove;

            var updatedRooms = this.Rooms.Select(r => r.Index == room.Index
                                                          ? updatedRoom
                                                          : r.Clone())
                                   .ToArray();

            possibleNewBurrows.Add((new Burrow
                                    {
                                        Hallway = updatedHallway,
                                        Rooms = updatedRooms
                                    }, moveEnergy));
        }

        return possibleNewBurrows.ToArray();
    }

    protected bool Equals(Burrow other)
    {
        return Rooms.SequenceEqual(other.Rooms) && Hallway.SequenceEqual(other.Hallway);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null,
                            obj))
        {
            return false;
        }

        if (ReferenceEquals(this,
                            obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((Burrow)obj);
    }

    public override int GetHashCode()
    {
        var hallwayString = string.Join(string.Empty,
                                        Hallway);

        var roomString = string.Join(string.Empty,
                                     Rooms.Select(x => $"{x.Top}{x.Bottom}"));

        return HashCode.Combine(hallwayString,
                                roomString);
    }
}

class Room
{
    public MapItem Top { get; set; }

    public MapItem Bottom { get; set; }

    public int Index { get; set; }

    public bool IsEmpty()
    {
        return Top == MapItem.Empty && Bottom == MapItem.Empty;
    }

    public bool HasSpace()
    {
        // Only need to check the top here, can't have a case where bottom is empty but top isn't
        return Top == MapItem.Empty;
    }

    public Room Clone()
    {
        return new Room
               {
                   Top = this.Top,
                   Bottom = this.Bottom,
                   Index = this.Index
               };
    }

    protected bool Equals(Room other)
    {
        return Top == other.Top && Bottom == other.Bottom && Index == other.Index;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null,
                            obj))
        {
            return false;
        }

        if (ReferenceEquals(this,
                            obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((Room)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Top,
                                (int)Bottom,
                                Index);
    }
}

enum MapItem
{
    Empty = 0,

    Amber = 1,

    Bronze = 2,

    Copper = 3,

    Desert = 4,

    Wall = 5
}