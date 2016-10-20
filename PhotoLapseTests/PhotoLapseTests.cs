using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PhotoLapseTools.Creators;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace PhotoLapseTests
{
    [TestClass]
    public class PhotoLapseTests
    {
        private static List<string> images = new List<string>() { "ff0000.png", "ffff00.png", "00ff00.png", "00ffff.png", "0000ff.png", "ff00ff.png" };
        private static string imagesPath = "../../images/";

        [TestMethod]
        public void TestStripes()
        {
            Test(new StripePhotoLapseCreator(), "../../images/stripe1.bmp");   
        }

        [TestMethod]
        public void TestGradient()
        {
            Test(new GradientPhotoLapseCreator(), "../../images/gradient1.bmp");
        }

        public void Test(IPhotoLapseCreator creator, string expected)
        {
            List<string> fullPath = images.Select(i => imagesPath + i).ToList();
            using (Bitmap resultBmp = creator.Process(fullPath))
            using (Bitmap expectedBmp = new Bitmap(expected))
            {
                Assert.IsTrue(Compare(resultBmp, expectedBmp, 1));
            }
        }

        public void Generate(IPhotoLapseCreator creator, string output)
        {
            List<string> fullPath = images.Select(i => imagesPath + i).ToList();
            using (Bitmap result = creator.Process(fullPath))
            {
                result.Save(output);
            }
        }

        private static bool Compare(Bitmap actual, Bitmap expected, int tolerance)
        {
            if (actual.Width != expected.Width) return false;
            if (actual.Height != expected.Height) return false;
            BitmapData bmdAct = actual.LockBits(new Rectangle(0, 0, actual.Width, actual.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmdExp = expected.LockBits(new Rectangle(0, 0, actual.Width, actual.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int wMul3 = actual.Width * 3;
            int h = actual.Height;
            int x, y;
            bool result = true;

            unsafe
            {
                for (y = 0; y < h; ++y)
                {
                    // Get row
                    byte* rowAct = (byte*)bmdAct.Scan0 + (y * bmdAct.Stride);
                    byte* rowExp = (byte*)bmdExp.Scan0 + (y * bmdExp.Stride);
                    // Iterate through columns
                    for (x = 0; x < wMul3; ++x)
                    {
                        if (Math.Abs(rowAct[x] - rowExp[x]) > tolerance)
                        {
                            result = false;
                        }
                    }
                }
            }

            actual.UnlockBits(bmdAct);
            expected.UnlockBits(bmdExp);
            return result;
        }
    }
}
