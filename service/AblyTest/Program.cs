using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace AblyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger(null, "testservice", Guid.Parse("9c4a890f76eb4de79c7d464e1375a502"));
            for (int i = 0; i != 5; ++i)
            {
                logger.Trace("Test " + i);
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }
    }
}
