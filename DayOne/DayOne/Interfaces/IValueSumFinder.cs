namespace DayOne.Interfaces
{
    public interface IValueSumFinder
    {
        #region Instance Methods

        /// <summary>
        /// Gets the sum for two values <paramref name="valueOne" /> and <paramref name="valueTwo" />.
        /// </summary>
        /// <param name="valueOne">The first value.</param>
        /// <param name="valueTwo">The second value.</param>
        /// <returns>The sum of these values.</returns>
        int GetSum(int valueOne,
                   int valueTwo);

        #endregion
    }
}