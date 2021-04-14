using System;

namespace Caching.CLI
{
    class Program
    {
        // Given a function f(x: int) -> int without side effects and whose
        // output is only dependent on it's single parameter — please produce a
        // function g(x: int) -> int which produces the same output as f for
        // every value of x but uses caching to improve performance.

        private static readonly DataCache<int> _cache = new DataCache<int>();

        static void Main(string[] args)
        {
            for (var j = 0; j < 5; j++)
            {
                for (var i = 0; i < 5; i++)
                {
                    var value = g(i);

                    Console.WriteLine($"f({i}) = {value}");
                }
            }
        }

        private static int f(int x)
        {
            return 2 * x;
        }

        private static int g(int x)
        {
            return _cache.GetOrAdd(x, x => f(x));
        }
    }
}
