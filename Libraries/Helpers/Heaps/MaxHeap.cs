using System.Collections.Generic;

using Priority_Queue;

namespace Helpers.Heaps
{
    public static class MaxHeap
    {
        #region Class Methods

        public static IPriorityQueue<T, int> CreateMaxHeap<T>()
        {
            return new SimplePriorityQueue<T, int>(new InverseComparer());
        }

        #endregion

        #region Nested type: InverseComparer

        public class InverseComparer : IComparer<int>
        {
            #region Instance Methods

            public int Compare(int x,
                               int y)
            {
                return y - x;
            }

            #endregion
        }

        #endregion
    }
}