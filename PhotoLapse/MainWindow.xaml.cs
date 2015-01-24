using Microsoft.Win32;
using System.Windows;

namespace PhotoLapse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Manager instance
        /// </summary>
        public PhotoLapseManager PLManager { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            PLManager = new PhotoLapseManager();
            this.DataContext = PLManager;
        }

        // Load button
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                PLManager.LoadPhotos(ofd.FileNames);
            }
        }

        // Render button
        private void btnRender_Click(object sender, RoutedEventArgs e)
        {
            PLManager.Render(rbGradient.IsChecked == true ? PhotoLapseType.Gradient : PhotoLapseType.Stripes);
        }

        // Save button
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = PLManager.DefaultFileName;
            sfd.Filter = "JPG image (*.jpg; *.jpeg)|*.jpg; *.jpeg";
            if (sfd.ShowDialog() == true)
            {
                long quality = long.Parse(txtQuality.Text);
                PLManager.SaveResult(sfd.FileName, quality);
            }
        }
        
        // Select all button
        private void lblSelectAll_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PLManager.SelectAll();
        }

        // Select none button
        private void lblSelectNone_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PLManager.SelectNone();
        }

        private void lblordName_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PLManager.OrderImages(OrderBy.Name);
        }

        private void lblordDate_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PLManager.OrderImages(OrderBy.Date);
        }
        
    }
}
