using PhotoLapse.Reporters;
using System.Collections.Generic;
using System.Drawing;

namespace PhotoLapse.Creators
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
        Bitmap Process(List<string> images, IReporter reporter = null);

        /// <summary>
        /// Create photolapse with weights
        /// </summary>
        /// <param name="images">List of images</param>
        /// <param name="weights">List of weights for each image</param>
        /// <param name="reporter">Reporter</param>
        /// <returns>Photolapse</returns>
        Bitmap Process(List<string> images, List<float> weights, IReporter reporter = null);
    }
}
