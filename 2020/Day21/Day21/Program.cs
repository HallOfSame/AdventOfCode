using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day21
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            var fileLines = File.ReadAllLines("PuzzleInput.txt");

            var foods = new List<Food>();

            var ingredients = new Dictionary<string, Ingredient>();

            var allergens = new Dictionary<string, Allergen>();

            foreach (var line in fileLines)
            {
                var splitLine = line.Split('(');

                var ingredientNames = splitLine[0]
                    .Split(' ');

                var foodIngredients = new HashSet<Ingredient>();

                foreach (var ingredientName in ingredientNames)
                {
                    var trimmedName = ingredientName.Trim();

                    if (string.IsNullOrEmpty(trimmedName))
                    {
                        continue;
                    }

                    if (!ingredients.TryGetValue(trimmedName,
                                                 out var ingredient))
                    {
                        ingredient = new Ingredient(trimmedName);
                        ingredients[trimmedName] = ingredient;
                    }

                    foodIngredients.Add(ingredient);
                }

                var allergenNames = splitLine[1][8..]
                                    .TrimEnd(')')
                                    .Split(',');

                var foodAllergens = new HashSet<Allergen>();

                foreach (var allergenName in allergenNames)
                {
                    var trimmedName = allergenName.Trim();

                    if (string.IsNullOrEmpty(trimmedName))
                    {
                        continue;
                    }

                    if (!allergens.TryGetValue(trimmedName,
                                               out var allergen))
                    {
                        allergen = new Allergen(trimmedName);
                        allergens[trimmedName] = allergen;
                    }

                    foodAllergens.Add(allergen);
                }

                foods.Add(new Food(foodIngredients,
                                   foodAllergens));
            }

            bool updatedAnIngredient;

            do
            {
                updatedAnIngredient = false;

                // Loop through the allergens
                foreach (var allergen in allergens.Values)
                {
                    // Find ingredients where every food with this allergen has this ingredient
                    var ingredientsInAllFoodWithThisAllergen = ingredients.Values.Where(i => foods.Where(f => f.Allergens.Contains(allergen))
                                                                                                  .All(f => f.Ingredients.Contains(i)))
                                                                          .ToList();

                    // If only one of those ingredients doesn't have a match use it
                    var ingredientToMatchUp = ingredientsInAllFoodWithThisAllergen.Where(x => x.Allergen == null)
                                                                                  .ToList();

                    if (ingredientToMatchUp.Count == 1)
                    {
                        updatedAnIngredient = true;
                        ingredientToMatchUp[0]
                            .Allergen = allergen;
                    }
                }

                // Loop until we go through and don't update any new items
            }
            while (updatedAnIngredient);

            var ingredientsWithNoAllergens = ingredients.Values.Where(x => x.Allergen == null)
                                                        .ToHashSet();

            var ingredientCount = ingredientsWithNoAllergens.Select(x => foods.Count(f => f.Ingredients.Contains(x)))
                                                            .Sum();

            //PT 1
            Console.WriteLine($"Appearances of non allergen ingredients: {ingredientCount}.");

            //PT 2
            var dangerImminentList = ingredients.Values.Where(x => x.Allergen != null)
                                                .OrderBy(x => x.Allergen.Name)
                                                .Select(x => x.Name)
                                                .ToList();

            Console.WriteLine("Danger List:");
            Console.WriteLine(string.Join(',',
                                          dangerImminentList));
        }

        #endregion
    }

    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class Ingredient
    {
        #region Constructors

        public Ingredient(string name)
        {
            Name = name;
        }

        #endregion

        #region Instance Properties

        public Allergen Allergen { get; set; }

        public string Name { get; }

        #endregion

        #region Instance Methods

        public override bool Equals(object obj)
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

            return Equals((Ingredient)obj);
        }

        public override int GetHashCode()
        {
            return Name != null
                       ? Name.GetHashCode()
                       : 0;
        }

        protected bool Equals(Ingredient other)
        {
            return Name == other.Name;
        }

        #endregion
    }

    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class Allergen
    {
        #region Constructors

        public Allergen(string name)
        {
            Name = name;
        }

        #endregion

        #region Instance Properties

        public string Name { get; }

        #endregion

        #region Instance Methods

        public override bool Equals(object obj)
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

            return Equals((Allergen)obj);
        }

        public override int GetHashCode()
        {
            return Name != null
                       ? Name.GetHashCode()
                       : 0;
        }

        protected bool Equals(Allergen other)
        {
            return Name == other.Name;
        }

        #endregion
    }

    public class Food
    {
        #region Constructors

        public Food(HashSet<Ingredient> ingredients,
                    HashSet<Allergen> allergens)
        {
            Ingredients = ingredients;
            Allergens = allergens;
        }

        #endregion

        #region Instance Properties

        public HashSet<Allergen> Allergens { get; }

        public HashSet<Ingredient> Ingredients { get; }

        #endregion
    }
}