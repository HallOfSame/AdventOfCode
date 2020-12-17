using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day06
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            var fileLines = File.ReadAllLines("PuzzleInput.txt");

            var planetDictionary = new Dictionary<string, Planet>();

            foreach (var line in fileLines)
            {
                var lineSplit = line.Split(')');

                var name = lineSplit[0];

                var orbiter = lineSplit[1];

                if (!planetDictionary.TryGetValue(name,
                                                  out var planet))
                {
                    planet = new Planet(name,
                                        null);
                    planetDictionary[name] = planet;
                }

                if (!planetDictionary.TryGetValue(orbiter,
                                                  out var orbitingPlanet))
                {
                    orbitingPlanet = new Planet(orbiter,
                                                planet);
                    planetDictionary[orbiter] = orbitingPlanet;
                }

                if (orbitingPlanet.DirectParent == null)
                {
                    // Depending on input order we might not have done set this up before
                    orbitingPlanet.DirectParent = planet;
                }

                planet.DirectOrbits.Add(orbitingPlanet);
            }

            // PT 1
            var orbits = planetDictionary.Values.Select(x => x.DirectOrbits.Count + x.IndirectOrbits.Count)
                                         .Sum();

            Console.WriteLine($"Total orbits {orbits}.");

            //PT 2

            var you = planetDictionary["YOU"];

            var yourCurrentOrbit = you.DirectParent;

            var santa = planetDictionary["SAN"];

            var santaCurrentOrbit = santa.DirectParent;

            bool CanStartMovingDown(Planet currentPlanet)
            {
                return currentPlanet.DirectOrbits.Any(x => x.Name == santaCurrentOrbit.Name) || currentPlanet.IndirectOrbits.Any(x => x.Name == santaCurrentOrbit.Name);
            }

            var yourPlanet = yourCurrentOrbit;

            var moves = 0;

            // Keep going up until we find a planet that we can get to Santa from
            while (!CanStartMovingDown(yourPlanet))
            {
                yourPlanet = yourPlanet.DirectParent;
                moves++;
            }

            Planet GetNextPlanetToHeadTo(Planet currentPlanet)
            {
                var directOption = currentPlanet.DirectOrbits.FirstOrDefault(x => x.Name == santaCurrentOrbit.Name);

                if (directOption != null)
                {
                    return directOption;
                }

                var indirectOption = currentPlanet.DirectOrbits.FirstOrDefault(x => x.DirectOrbits.Concat(x.IndirectOrbits)
                                                                                     .Any(y => y.Name == santaCurrentOrbit.Name));

                if (indirectOption != null)
                {
                    return indirectOption;
                }

                throw new InvalidOperationException("No planet found to go to.");
            }

            while (yourPlanet.Name != santaCurrentOrbit.Name)
            {
                yourPlanet = GetNextPlanetToHeadTo(yourPlanet);
                moves++;
            }

            Console.WriteLine($"Moves to Santa {moves}.");
        }

        #endregion
    }
}