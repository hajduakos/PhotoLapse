using PhotoLapseTools.Creators;
using PhotoLapseTools.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace PhotoLapse
{
    /// <summary>
    /// Class managing the loading and rendering process.
    /// </summary>
    public class PhotoLapseManager : INotifyPropertyChanged
    {
        private System.Drawing.Bitmap bmpResult;       // Rendering result (RAW)
        private BackgroundWorker bwRenderer, bwLoader; // Backgroundworkers for rendering and loading

        /// <summary>
        /// List of loaded photos
        /// </summary>
        public ObservableCollection<Photo> Photos { get; private set; }

        /// <summary>
        /// Rendering result
        /// </summary>
        public BitmapSource Result { get; private set; }

        /// <summary>
        /// Is there any loading or rendering
        /// </summary>
        public bool IsIdle { get; private set; }

        /// <summary>
        /// Percentage of loading or rendering
        /// </summary>
        public int ProgressPercentage { get; private set; }

        /// <summary>
        /// Message to be displayed
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public PhotoLapseManager()
        {
            // Initialize variables
            Photos = new ObservableCollection<Photo>(); 
            Result = null; RaisePropertyChanged("Result");
            IsIdle = true; RaisePropertyChanged("IsIdle");
            ProgressPercentage = 0; RaisePropertyChanged("ProgressPercentage");
            Message = ""; RaisePropertyChanged("Message");
            bmpResult = null;

            // Initialize backgroundworkers
            bwRenderer = new BackgroundWorker();
            bwRenderer.WorkerReportsProgress = true;
            bwRenderer.DoWork += bwRenderer_DoWork;
            bwRenderer.ProgressChanged += bwRenderer_ProgressChanged;
            bwRenderer.RunWorkerCompleted += bwRenderer_RunWorkerCompleted;

            bwLoader = new BackgroundWorker();
            bwLoader.WorkerReportsProgress = true;
            bwLoader.DoWork += bwLoader_DoWork;
            bwLoader.ProgressChanged += bwLoader_ProgressChanged;
            bwLoader.RunWorkerCompleted += bwLoader_RunWorkerCompleted;
        }

        /// <summary>
        /// Load photos given by an array of file names
        /// </summary>
        /// <param name="fileNames">Array of file names</param>
        public void LoadPhotos(string[] fileNames)
        {
            // Clear list and start loading worker
            Photos.Clear();
            IsIdle = false; RaisePropertyChanged("IsIdle");
            bwLoader.RunWorkerAsync(fileNames);
        }

        /// <summary>
        /// Render
        /// </summary>
        /// <param name="type">Type of the photolapse</param>
        public void Render(PhotoLapseType type)
        {
            // Clear previous result to save memory
            Result = null; RaisePropertyChanged("Result");
            if (bmpResult != null) bmpResult.Dispose();
            // Initialize creator and start rendering worker
            IPhotoLapseCreator creator = new StripePhotoLapseCreator();
            if (type == PhotoLapseType.Gradient) creator = new GradientPhotoLapseCreator();
            IsIdle = false; RaisePropertyChanged("IsIdle");
            bwRenderer.RunWorkerAsync(creator);
        }

        /// <summary>
        /// Save result
        /// </summary>
        /// <param name="fileName">File name of result</param>
        /// <param name="quality">JPG compression quality (0-100)</param>
        public void SaveResult(string fileName, long quality)
        {
            PhotoUtils.SaveJpg(bmpResult, fileName, quality);
        }

        /// <summary>
        /// Select every photo in the list
        /// </summary>
        public void SelectAll()
        {
            foreach (Photo p in Photos) p.IsSelected = true;
        }

        /// <summary>
        /// Unselect every photo in the list
        /// </summary>
        public void SelectNone()
        {
            foreach (Photo p in Photos) p.IsSelected = false;
        }

        // Loading process
        private void bwLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            // Sort and load images
            bwLoader.ReportProgress(0);
            List<string> sorted = new List<string>(e.Argument as string[]);
            sorted.Sort();
            for (int i = 0; i < sorted.Count; i++)
            {
                // Pass the loaded photo as user state
                bwLoader.ReportProgress((int)((i + 1) / (float)(sorted.Count) * 100), new Photo(sorted[i]));
            }
            bwLoader.ReportProgress(100);
        }

        // Loading progress changed
        private void bwLoader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Set percentage and message
            ProgressPercentage = e.ProgressPercentage; RaisePropertyChanged("ProgressPercentage");
            Message = "Loading " + e.ProgressPercentage + "%"; RaisePropertyChanged("Message");
            // Retrieve photo from user state and add to list
            if (e.UserState != null) Photos.Add(e.UserState as Photo);
        }

        // Loading complete
        private void bwLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsIdle = true; RaisePropertyChanged("IsIdle");
            Message = "Loading complete!"; RaisePropertyChanged("Message");
        }

        // Rendering process
        private void bwRenderer_DoWork(object sender, DoWorkEventArgs e)
        {
            // Filter only selected photos
            IPhotoLapseCreator creator = e.Argument as IPhotoLapseCreator;
            bmpResult = creator.Process(
                Photos.Where(p => p.IsSelected).Select(p => p.Path).ToList(),
                new BgWorkerReporter(bwRenderer));
        }

        // Rendering progress changed
        private void bwRenderer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage; RaisePropertyChanged("ProgressPercentage");
            Message = "Rendering " + e.ProgressPercentage + "%"; RaisePropertyChanged("Message");
        }

        // Rendering complete
        private void bwRenderer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Result = BitmapToBitmapSource(bmpResult); RaisePropertyChanged("Result");
            IsIdle = true; RaisePropertyChanged("IsIdle");
            Message = "Rendering complete!"; RaisePropertyChanged("Message");
        }
        
        // Helper function to convert Bitmap to BitmapSource
        private BitmapSource BitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        // Helper function for INPC
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
