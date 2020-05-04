using PhotoLapse.Reporters;
using System.ComponentModel;

namespace PhotoLapse
{
    /// <summary>
    /// Adapter class for backgroundworker based reporting
    /// </summary>
    class BgWorkerReporter : IReporter
    {
        private readonly BackgroundWorker worker;

        /// <summary>
        /// Report progress
        /// </summary>
        /// <param name="done">Number of images processed</param>
        /// <param name="count">Number of total images</param>
        public BgWorkerReporter(BackgroundWorker worker)
        {
            this.worker = worker;
        }

        /// <summary>
        /// Report progress
        /// </summary>
        /// <param name="done">Number of images processed</param>
        /// <param name="count">Number of total images</param>
        public void Report(int done, int count)
        {
            worker.ReportProgress((int)(done / (float)count * 100));
        }
    }
}
