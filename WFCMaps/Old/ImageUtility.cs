using System;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace WFC.Utils
{
    /// <summary>
    /// The <see cref="ImageUtility"/> is a utility class for loading and processing sample images for use in the WFC.
    /// </summary>
    public static class ImageUtility
    {
        private static Image<Rgba32> image;
        private static char[,] processedImage;

        private static Rgba32 INDOOR_COLOR = new Rgba32(178, 178, 178, 255);
        private static Rgba32 OUTDOOR_COLOR = new Rgba32(255, 255, 255, 255);
        private static Rgba32 WALL_COLOR = new Rgba32(0, 0, 0, 255);

        public static Image<Rgba32> GetImage { get => image; }
        public static char[,] GetProcessedImage { get => processedImage; }


        /// <summary>
        /// Converts an image into a 2D array of <see cref="char"/>s. Outputs to <see cref="GetProcessedImage"/>. 
        /// </summary>
        /// <param name="filename"></param>
        public static void ProcessImage(string filename)
        {
            image = Image.Load(filename) as Image<Rgba32>;
            processedImage = new char[image.Height, image.Width];

            for (int y = 0; y < image.Height; y++)
            {
                Span<Rgba32> span = image.GetPixelRowSpan(y);
                for (int x = 0; x < image.Width; x++)
                {
                    if (span[x] == INDOOR_COLOR)
                    {
                        processedImage[y, x] = 'I';
                    }
                    else if (span[x] == OUTDOOR_COLOR)
                    {
                        processedImage[y, x] = 'O';
                        //processedImage[y, x] = random.Next(10) > 3 ? 'O' : 'X';
                    }
                    else if (span[x] == WALL_COLOR)
                    {
                        processedImage[y, x] = 'X';
                    }
                }
            }
        }

    }
}
