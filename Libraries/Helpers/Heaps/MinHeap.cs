using Priority_Queue;

namespace Helpers.Heaps
{
    public static class MinHeap
    {
        #region Class Methods

        public static IPriorityQueue<T, int> CreateMinHeap<T>()
        {
            return new SimplePriorityQueue<T, int>();
        }

        #endregion
    }
}