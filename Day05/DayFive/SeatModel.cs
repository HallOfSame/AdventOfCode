using System;

namespace DayFive
{
    public class SeatModel
    {
        #region Constructors

        public SeatModel(int row,
                         int column)
        {
            Row = row;
            Column = column;
        }

        #endregion

        #region Instance Properties

        public int Column { get; }

        public int Row { get; }

        public int SeatId
        {
            get
            {
                return Row * 8 + Column;
            }
        }

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

            return Equals((SeatModel)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Column,
                                    Row);
        }

        protected bool Equals(SeatModel other)
        {
            return Column == other.Column && Row == other.Row;
        }

        #endregion
    }
}