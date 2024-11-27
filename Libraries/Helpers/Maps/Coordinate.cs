using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.Maps
{
    public class Coordinate : ICloneable
    {
        #region Constructors

        public Coordinate()
        {
        }

        public Coordinate(decimal x,
                          decimal y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Instance Properties

        public decimal X { get; set; }

        public decimal Y { get; set; }

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

            return Equals((Coordinate)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X,
                                    Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public object Clone()
        {
            return new Coordinate(X,
                                  Y);
        }

        protected bool Equals(Coordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        #endregion

        #region Class Methods

        public static bool operator ==(Coordinate left,
                                       Coordinate right)
        {
            return Equals(left,
                          right);
        }

        public static bool operator !=(Coordinate left,
                                       Coordinate right)
        {
            return !Equals(left,
                           right);
        }

        #endregion
    }

    public class CoordinateConverter : JsonConverter<Coordinate>
    {
        public override Coordinate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Expected a string.");
            }

            var parts = reader.GetString()!.Split(',');
            if (parts.Length != 2 || 
                !decimal.TryParse(parts[0], out var x) || 
                !decimal.TryParse(parts[1], out var y))
            {
                throw new JsonException("Invalid format for Coordinate.");
            }

            return new Coordinate { X = x, Y = y };
        }

        public override void Write(Utf8JsonWriter writer, Coordinate value, JsonSerializerOptions options)
        {
            var serializedValue = $"{value.X},{value.Y}";
            writer.WriteStringValue(serializedValue);
        }
    }
}