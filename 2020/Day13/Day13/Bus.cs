namespace Day13
{
    public class Bus
    {
        #region Constructors

        public Bus(long nextDeparture,
                   long id)
        {
            NextDeparture = nextDeparture;
            Id = id;
        }

        #endregion

        #region Instance Properties

        public long Id { get; }

        public long NextDeparture { get; }

        #endregion
    }
}