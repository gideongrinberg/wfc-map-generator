using System;
using DeBroglie.Topo;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using WFC;

namespace WFC.Utils
{
    public static class ExportUtility
    {
        public static void ToImage(string filename, WaveFunctionCollapse wfc, bool useUniqueName = true)
        {
            filename = useUniqueName ? GenerateFileName(filename) : filename;

            Image<Rgba32> image = new Image<Rgba32>(wfc.GetWidth, wfc.GetHeight);
            ITopoArray<char> output = wfc.GetPropagator.ToValueArray<char>();

            for (var y = 0; y < wfc.GetWidth; y++)
            {
                for (var x = 0; x < wfc.GetWidth; x++)
                {
                    switch (output.Get(x, y))
                    {
                        case 'I':
                            image[x, y] = wfc.INDOOR_COLOR;
                            break;
                        case 'O':
                            image[x, y] = wfc.OUTDOOR_COLOR; //image[x, y] = random.Next(10) > 3 ? OUTDOOR_COLOR : WALL_COLOR;
                            break;
                        case 'X':
                            image[x, y] = wfc.WALL_COLOR;
                            break;
                        default:
                            break;
                    }
                }
            }

            image.SaveAsPng(filename);
            Console.WriteLine($"Wrote image output to {filename}");
        }

        public static string GenerateFileName(string context)
        {
            return context + "_" + DateTime.Now.ToFileTimeUtc() + ".png";
        }

    }
}
