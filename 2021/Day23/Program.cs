using Helpers.Structure;

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
                                               Inhabitants = new []
                                                            {
                                                                MapItem.Desert,
                                                                MapItem.Bronze
                                                            }
                                           },
                                           new()
                                           {
                                               Index = 4,
                                               Inhabitants = new []
                                                             {
                                                                 MapItem.Desert,
                                                                 MapItem.Amber
                                                             }
                                           },
                                           new()
                                           {
                                               Index = 6,
                                               Inhabitants = new []
                                                             {
                                                                 MapItem.Copper,
                                                                 MapItem.Amber
                                                             }
                                           },
                                           new()
                                           {
                                               Index = 8,
                                               Inhabitants = new []
                                                             {
                                                                 MapItem.Bronze,
                                                                 MapItem.Copper
                                                             }
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
        var updatedPosition = new Burrow
                              {
                                  Hallway = (MapItem[])startingPosition.Hallway.Clone(),
                                  Rooms = startingPosition.Rooms.Select((originalRoom,
                                                                         idx) =>
                                                                        {
                                                                            var newInhabitants = new MapItem[4];

                                                                            newInhabitants[0] = originalRoom.Inhabitants[0];

                                                                            newInhabitants[3] = originalRoom.Inhabitants[1];

                                                                            var idxOne = MapItem.Empty;
                                                                            var idxTwo = MapItem.Empty;

                                                                            switch (idx)
                                                                            {
                                                                                case 0:
                                                                                    idxOne = MapItem.Desert;
                                                                                    idxTwo = MapItem.Desert;
                                                                                    break;
                                                                                case 1:
                                                                                    idxOne = MapItem.Copper;
                                                                                    idxTwo = MapItem.Bronze;
                                                                                    break;
                                                                                case 2:
                                                                                    idxOne = MapItem.Bronze;
                                                                                    idxTwo = MapItem.Amber;
                                                                                    break;
                                                                                case 3:
                                                                                    idxOne = MapItem.Amber;
                                                                                    idxTwo = MapItem.Copper;
                                                                                    break;
                                                                            }

                                                                            newInhabitants[1] = idxOne;
                                                                            newInhabitants[2] = idxTwo;

                                                                            return new Room
                                                                                   {
                                                                                       Index = originalRoom.Index,
                                                                                       Inhabitants = newInhabitants
                                                                                   };
                                                                        })
                                                          .ToArray()
                              };

        var solve = new BurrowSolver();

        var energyNeeded = solve.GetEnergyRequired(updatedPosition);

        return energyNeeded.ToString();
    }

    #endregion
}

class BurrowSolver
{
    public int GetEnergyRequired(Burrow start)
    {
        var roomSize = start.Rooms[0]
                            .Inhabitants.Length;

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
                                       Inhabitants = Enumerable.Repeat(MapItem.Amber,
                                                                       roomSize)
                                                               .ToArray()
                                   },
                                   new()
                                   {
                                       Index = 4,
                                       Inhabitants = Enumerable.Repeat(MapItem.Bronze,
                                                                       roomSize)
                                                               .ToArray()
                                   },
                                   new()
                                   {
                                       Index = 6,
                                       Inhabitants = Enumerable.Repeat(MapItem.Copper,
                                                                       roomSize)
                                                               .ToArray()
                                   },
                                   new()
                                   {
                                       Index = 8,
                                       Inhabitants = Enumerable.Repeat(MapItem.Desert,
                                                                       roomSize)
                                                               .ToArray()
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
        // About 100 possible moves total, but making a list that large is inefficient
        var possibleMoves = new List<(Burrow, int)>(35);

        // Moving something out of a room:
        foreach (var room in Rooms)
        {
            // If the room is empty or correct, we don't need to keep checking it
            if (room.IsEmpty() || room.IsCorrect())
            {
                continue;
            }

            possibleMoves.AddRange(GetMoveOut(room));

            var possibleRoomToRoomMove = GetMoveOutToOtherRoom(room);

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

    public (Burrow, int)? GetMoveOutToOtherRoom(Room room)
    {
        var itemToMove = room.Inhabitants.First(x => x != MapItem.Empty);

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
            || destinationRoom.Inhabitants.Any(x => x != itemToMove && x != MapItem.Empty))
        {
            // It's not valid to move this item back in yet
            // The room is full, or has the wrong type still in it
            return null;
        }

        var updatedHallway = (MapItem[])Hallway.Clone();

        var index = Array.LastIndexOf(destinationRoom.Inhabitants,
                                  MapItem.Empty);

        var updatedDestinationRoom = destinationRoom.Clone();

        updatedDestinationRoom.Inhabitants[index] = itemToMove;

        var sourceIndex = Array.IndexOf(room.Inhabitants,
                                        itemToMove);

        var updatedSourceRoom = room.Clone();

        updatedSourceRoom.Inhabitants[sourceIndex] = MapItem.Empty;

        var moveDistance = Math.Abs(room.Index - destinationRoom.Index);

        moveDistance += sourceIndex + 1;

        moveDistance += index + 1;

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

        if (!room.HasSpace() || room.Inhabitants.Any(x => x != itemToMove && x != MapItem.Empty))
        {
            // It's not valid to move this item back in yet
            // The room is full, or has the wrong type still in it
            return null;
        }

        var updatedHallway = (MapItem[])Hallway.Clone();

        updatedHallway[hallwayIndex] = MapItem.Empty;

        var emptyIndex =  Array.LastIndexOf(room.Inhabitants, MapItem.Empty);

        var updatedRoom = room.Clone();

        updatedRoom.Inhabitants[emptyIndex] = itemToMove;

        var moveDistance = Math.Abs(roomIndex - hallwayIndex);

        moveDistance += emptyIndex + 1;

        var energyUsed = GetMoveEnergy(moveDistance,
                                       itemToMove);

        return (new Burrow
                {
                    Hallway = updatedHallway,
                    Rooms = GetUpdatedRooms(updatedRoom)
                }, energyUsed);
    }

    private Room[] GetUpdatedRooms(Room changedRoom)
    {
        return this.Rooms.Select(r => r.Index == changedRoom.Index
                                          ? changedRoom
                                          : r)
                   .ToArray();
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
        var openHallwaySpots = new List<int>(11);

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

        var moveIndex = Array.FindIndex(room.Inhabitants,
                                        x => x != MapItem.Empty);

        var itemToMove = room.Inhabitants[moveIndex];

        var updatedRoom = room.Clone();

        updatedRoom.Inhabitants[moveIndex] = MapItem.Empty;

        var possibleNewBurrows = new List<(Burrow, int)>(openHallwaySpots.Count);

        foreach (var index in openHallwaySpots)
        {
            var moveDistance = Math.Abs(room.Index - index);

            moveDistance += moveIndex + 1;

            var moveEnergy = GetMoveEnergy(moveDistance,
                                           itemToMove);

            var updatedHallway = (MapItem[])Hallway.Clone();

            updatedHallway[index] = itemToMove;

            var updatedRooms = GetUpdatedRooms(updatedRoom);

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
        var hash = 0;

        foreach (var x in Hallway)
        {
            hash *= 17;
            hash += x.GetHashCode();
        }

        foreach (var z in Rooms)
        {
            hash *= 17;
            hash += z.GetHashCode();
        }

        return hash;
    }
}

class Room
{
    public MapItem[] Inhabitants { get; set; }

    public int Index { get; set; }

    public bool IsEmpty()
    {
        return Inhabitants.All(x => x == MapItem.Empty);
    }

    public bool HasSpace()
    {
        // Only need to check the top here, can't have a case where bottom is empty but top isn't
        return Inhabitants[0] == MapItem.Empty;
    }

    private MapItem ExpectedItem
    {
        get
        {
            return Index switch
            {
                2 => MapItem.Amber,
                4 => MapItem.Bronze,
                6 => MapItem.Copper,
                8 => MapItem.Desert
            };
        }
    }

    public bool IsCorrect()
    {
        return Inhabitants.All(x => x == ExpectedItem);
    }

    public Room Clone()
    {
        return new Room
               {
                   Inhabitants = (MapItem[])Inhabitants.Clone(),
                   Index = this.Index
               };
    }

    protected bool Equals(Room other)
    {
        return this.Inhabitants.SequenceEqual(other.Inhabitants);
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
        var hash = Index;

        foreach (var x in Inhabitants)
        {
            hash *= 17;
            hash += x.GetHashCode();
        }

        return hash;
    }
}

enum MapItem
{
    Empty = 0,

    Amber = 1,

    Bronze = 2,

    Copper = 3,

    Desert = 4
}