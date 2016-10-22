using PhotoLapseTools.Creators;
using PhotoLapseTools.Utils;
using System;
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
        private OrderBy order;                         // Current order of images
        private bool isAscending;                      // Asc. or desc.

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
        /// Length of the interval in which the photos were taken
        /// </summary>
        public TimeSpan TimeSpan
        {
            get
            {
                if (Photos.Count < 2) return TimeSpan.FromSeconds(0);
                else return Photos.Max(p => p.DateModified) - Photos.Min(p => p.DateModified);
            }
        }

        /// <summary>
        /// Get the default filename for exporting
        /// </summary>
        public string DefaultFileName
        {
            get
            {
                string fileName = "";
                Photo first = Photos.Where(p => p.IsSelected).FirstOrDefault();
                if (first != null) fileName += first.Name + "_";
                fileName += "photolapse_" + Photos.Count(p => p.IsSelected) + "_images.jpg";
                return fileName;
            }
        }

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
            order = OrderBy.Name;
            isAscending = true;

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

        /// <summary>
        /// Order images
        /// </summary>
        /// <param name="orderBy">Property that is ordered</param>
        public void OrderImages(OrderBy orderBy)
        {
            if (this.order == orderBy) isAscending = !isAscending;
            else
            {
                this.order = orderBy;
                isAscending = true;
            }

            OrderImages();
        }

        // Helper function for ordering images
        private void OrderImages()
        {
            List<Photo> temp;

            switch (order)
            {
                case OrderBy.Name:
                    temp = Photos.OrderBy(p => p.Name).ToList();
                    break;
                default:
                    temp = Photos.OrderBy(p => p.Name).ToList();
                    break;
            }
            if (!isAscending) temp.Reverse();
            Photos = new ObservableCollection<Photo>(temp);
            RaisePropertyChanged("Photos");
        }

        // Loading process
        private void bwLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            // Sort and load images
            bwLoader.ReportProgress(0);
            string[] files = e.Argument as string[];
            for (int i = 0; i < files.Length; i++)
            {
                // Pass the loaded photo as user state
                bwLoader.ReportProgress((int)((i + 1) / (float)(files.Length) * 100), new Photo(files[i]));
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
            RaisePropertyChanged("TimeSpan");
        }

        // Loading complete
        private void bwLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OrderImages();
            IsIdle = true; RaisePropertyChanged("IsIdle");
            Message = "Loading complete!"; RaisePropertyChanged("Message");
            RaisePropertyChanged("TimeSpan");
        }

        // Rendering process
        private void bwRenderer_DoWork(object sender, DoWorkEventArgs e)
        {
            // Filter only selected photos
            IPhotoLapseCreator creator = e.Argument as IPhotoLapseCreator;
            List<string> photos = Photos.Where(p => p.IsSelected).Select(p => p.Path).ToList();
            List<float> weights = Photos.Where(p => p.IsSelected).Select(p => p.Weight).ToList();
            if (creator is GradientPhotoLapseCreator) weights.RemoveAt(weights.Count - 1);

            bmpResult = creator.Process(photos, weights, new BgWorkerReporter(bwRenderer));
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
