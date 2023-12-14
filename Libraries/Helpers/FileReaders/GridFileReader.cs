﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Helpers.Maps;

namespace Helpers.FileReaders;

public class GridFileReader
{
    public async Task<List<CoordinateWithCharacter>> ReadInputFromFile()
    {
        await using var file = File.OpenRead("PuzzleInput.txt");
        using var reader = new StreamReader(file);

        var strings = new List<string>();

        while (!reader.EndOfStream)
        {
            var nextLine = await reader.ReadLineAsync();

            strings.Add(nextLine);
        }

        // Reverse to make the origin the bottom left
        strings.Reverse();

        var strLength = strings[0]
            .Length;

        var outputs = new List<CoordinateWithCharacter>();

        for (var y = 0; y < strings.Count; y++)
        for (var x = 0; x < strLength; x++)
            outputs.Add(new CoordinateWithCharacter(new Coordinate(x, y))
            {
                Value = strings[y][x]
            });

        return outputs;
    }
}

public class CoordinateWithCharacter : ObjectWithCoordinateEquality
{
    public char Value { get; set; }

    public CoordinateWithCharacter(Coordinate coordinate) : base(coordinate)
    {
    }
}