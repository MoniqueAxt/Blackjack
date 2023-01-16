namespace Utilities
{
    /// <summary>
    /// Randomly arranges a list using the Fisher-Yates shuffle algorithm.
    /// </summary>
    public static class RandomArranger
    {
        private static readonly Random rnd = new();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
