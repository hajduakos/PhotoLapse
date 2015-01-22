using PhotoLapseTools.Creators;
using PhotoLapseTools.Reporters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace PhotoLapseConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get images from argument
            List<string> images = new List<string>(args);
            images.Sort();

            Console.WriteLine("PhotoLapseConsole");
            
            // Get PhotoLapse type (gradient or stripe)
            Console.Write("Gradient (g) or stripe (s)? ");
            string type = Console.ReadLine();

            IPhotoLapseCreator creator = null;

            if (type.ToLower() == "g") creator = new GradientPhotoLapseCreator();
            else creator = new StripePhotoLapseCreator();

            // Create and save image
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                Bitmap result = creator.Process(images, new ConsoleReporter());
                Console.WriteLine(sw.ElapsedMilliseconds + "ms");
                PhotoLapseTools.Utils.PhotoUtils.SaveJpg(result,
                    System.IO.Path.GetFileNameWithoutExtension(images[0]) + "_photolapse.jpg", 95);
                Console.WriteLine("Image saved!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured: " + ex.Message);
                Console.WriteLine("Press any key to exit");
            }

            Console.ReadKey();
        }
    }
}
