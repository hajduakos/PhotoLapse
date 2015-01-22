using System;

namespace PhotoLapseTools.Reporters
{
    /// <summary>
    /// Console reporter
    /// </summary>
    public class ConsoleReporter : IReporter
    {
        /// <summary>
        /// Report progress
        /// </summary>
        /// <param name="done">Number of images processed</param>
        /// <param name="count">Number of total images</param>
        public void Report(int done, int count)
        {
            Console.WriteLine("{0} of {1} photos processed.", done, count);
        }
    }
}
