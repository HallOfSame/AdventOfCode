namespace Day13
{
    public class Bus
    {
        #region Constructors

        public Bus(int nextDeparture,
                   int id)
        {
            NextDeparture = nextDeparture;
            Id = id;
        }

        #endregion

        #region Instance Properties

        public int Id { get; }

        public int NextDeparture { get; }

        #endregion
    }
}