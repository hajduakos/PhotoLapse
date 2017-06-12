
namespace PhotoLapse.Reporters
{
    /// <summary>
    /// Reporter interface
    /// </summary>
    public interface IReporter
    {
        /// <summary>
        /// Report progress
        /// </summary>
        /// <param name="done">Number of images processed</param>
        /// <param name="count">Number of total images</param>
        void Report(int done, int count);
    }
}
