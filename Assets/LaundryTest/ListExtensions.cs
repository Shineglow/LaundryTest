using System.Collections.Generic;

namespace LaundryTest
{
    public static class ListExtensions
    {
        public static void AlignCapacityWithCount<T>(this List<T> l1, List<T> l2)
        {
            if (l2.Count > l1.Capacity)
            {
                l1.Capacity = l2.Count;
            }
        }
    }
}