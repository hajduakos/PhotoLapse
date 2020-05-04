using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace PhotoLapse
{
    /// <summary>
    /// Represents a photo with file name, thumbnail and an indicator
    /// whether it is selected.
    /// </summary>
    public class Photo : INotifyPropertyChanged
    {
        /// <summary>
        /// Path
        /// </summary>
        public string Path { get; private set; }

        private bool isSelected;

        /// <summary>
        /// Is the photo selected for rendering
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; RaisePropertyChanged("IsSelected"); }
        }

        /// <summary>
        /// Filename without extension
        /// </summary>
        public string Name { get { return System.IO.Path.GetFileNameWithoutExtension(Path); } }

        /// <summary>
        /// Thumbnail image
        /// </summary>
        public BitmapImage Thumb { get; private set; }

        /// <summary>
        /// Date modified
        /// </summary>
        public DateTime DateModified { get; private set; }

        private float weight;
        public float Weight
        {
            get { return weight; }
            set { weight = Math.Max(value, 0); RaisePropertyChanged("Weight"); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path of the photo</param>
        public Photo(string path)
        {
            this.Path = path; RaisePropertyChanged("Path"); RaisePropertyChanged("Name");
            this.IsSelected = true; RaisePropertyChanged("IsSelected");
            this.DateModified = System.IO.File.GetLastWriteTime(Path); RaisePropertyChanged("DateModified");
            Thumb = new BitmapImage();
            Thumb.BeginInit();
            Thumb.CacheOption = BitmapCacheOption.OnLoad;
            Thumb.UriSource = new Uri(path);
            Thumb.DecodePixelHeight = 200;
            Thumb.EndInit();
            Thumb.Freeze(); RaisePropertyChanged("Thumb");
            this.weight = 1f; RaisePropertyChanged("Weight");
        }

        /// <summary>
        /// Event fired when a property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
