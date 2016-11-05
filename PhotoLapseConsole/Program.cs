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
            string type = null;
            List<string> images = new List<string>();
            List<float> weights = new List<float>();
            string output = null;
            int padding = 0;

            HashSet<string> opts = new HashSet<string>() { "-t", "-i", "-w", "-o", "-p" };

            try
            {
                for (int i = 0; i < args.Length; ++i)
                {
                    if (args[i] == "-t")
                    {
                        type = args[i + 1];
                    }
                    else if (args[i] == "-i")
                    {
                        ++i;
                        while (i < args.Length && !opts.Contains(args[i]))
                        {
                            images.Add(args[i]);
                            ++i;
                        }
                        --i;
                    }
                    else if (args[i] == "-w")
                    {
                        ++i;
                        while (i < args.Length && !opts.Contains(args[i]))
                        {
                            weights.Add(float.Parse(args[i]));
                            ++i;
                        }
                        --i;
                    }
                    else if (args[i] == "-o")
                    {
                        output = args[i + 1];
                    }
                    else if (args[i] == "-p")
                    {
                        padding = int.Parse(args[i + 1]);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error while parsing arguments.");
                PrintUsage();
                return;
            }

            if (type == null)
            {
                Console.WriteLine("Type is not specified.");
                PrintUsage();
                return;
            }
            if (!(type == "stripes" || type == "gradient"))
            {
                Console.WriteLine("Invalid type.");
                PrintUsage();
                return;
            }
            if (output == null)
            {
                Console.WriteLine("Output is not specified.");
                PrintUsage();
                return;
            }

            images.Sort();

            IPhotoLapseCreator creator = null;

            if (type.ToLower() == "gradient") creator = new GradientPhotoLapseCreator();
            else creator = new StripePhotoLapseCreator(padding);

            // Create and save image
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                Bitmap result  = null;
                if (weights.Count == 0)
                    result = creator.Process(images, new ConsoleReporter());
                else
                    result = creator.Process(images, weights, new ConsoleReporter());
                Console.WriteLine(sw.ElapsedMilliseconds + "ms");
                Console.Write("Saving...");
                PhotoLapseTools.Utils.PhotoUtils.SaveJpg(result, output, 95);
                Console.WriteLine("done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured: " + ex.Message);
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: PhotoLapseConsole.exe -t stripes|gradient -i images -o output [-w weights] [-p padding]");
            Console.WriteLine("Example: PhotoLapseConsole.exe -t stripes -i img1.jpg img2.jpg img3.jpg -o out.jpg -w 1 2 3 -p 10");
        }
    }
}
