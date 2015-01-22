
namespace PhotoLapseTools.Creators
{
    /// <summary>
    /// PhotoLapse creator interface
    /// </summary>
    public interface IPhotoLapseCreator
    {
        /// <summary>
        /// Create photolapse
        /// </summary>
        /// <param name="images">List of images</param>
        /// <param name="reporter">Reporter</param>
        /// <returns>Photolapse</returns>
        System.Drawing.Bitmap Process(System.Collections.Generic.List<string> images, Reporters.IReporter reporter = null);
    }
}
