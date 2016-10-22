using PhotoLapseTools.Reporters;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PhotoLapseTools.Creators
{
    /// <summary>
    /// Base class for photolapse creators
    /// </summary>
    public abstract class PhotoLapseCreatorBase : IPhotoLapseCreator
    {
        /// <summary>
        /// Create photolapse
        /// </summary>
        /// <param name="images">List of images</param>
        /// <param name="reporter">Reporter</param>
        /// <returns>Photolapse</returns>
        public Bitmap Process(List<string> images, IReporter reporter = null)
        {
            List<float> weights = images.Select(i => 1f).ToList();
            return Process(images, weights, reporter);
        }

        /// <summary>
        /// Create photolapse with weights
        /// </summary>
        /// <param name="images">List of images</param>
        /// <param name="weights">List of weights for each image</param>
        /// <param name="reporter">Reporter</param>
        /// <returns>Photolapse</returns>
        public abstract Bitmap Process(List<string> images, List<float> weights, IReporter reporter = null);
       
    }
}
