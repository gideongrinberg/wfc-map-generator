using System;

using DeBroglie;
using DeBroglie.Topo;
using DeBroglie.Models;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace WaveFunctionCollapse
{
    public class WFCGenerator
    {

        private Rgba32 INDOOR_COLOR = new Rgba32(178, 178, 178, 255);
        private Rgba32 OUTDOOR_COLOR = new Rgba32(255, 255, 255, 255);
        private Rgba32 WALL_COLOR = new Rgba32(0, 0, 0, 255);

        private TilePropagator propagator;
        public TilePropagator GetPropagator { get => propagator; }

        private OverlappingModel model;
        public OverlappingModel GetModel { get => model; }

        public ITopoArray<char> sample;

        private int width;
        private int height;

        public WFCGenerator(int n = 3, int width = 90, int height = 90, string inputFilename = "./input.png", string outputFilename = "./output.png", bool overlapping = true)
        {
            this.width = width;
            this.height = height;

            sample = TopoArray.Create(ImageToArray(inputFilename), false);

            model = new OverlappingModel(sample.ToTiles(), n, 4, true);

            GridTopology topology = new GridTopology(width, height, false);
            propagator = new TilePropagator(model, topology);
        }

        public ITopoArray<char> Run()
        {
            Resolution status = propagator.Run();
            if (status != Resolution.Decided) throw new Exception("Undecided");

            return propagator.ToValueArray<char>();
        }

        public char[,] ImageToArray(string filename)
        {
            using (Image<Rgba32> image = (Image<Rgba32>)Image.Load(filename))
            {
                char[,] processedImage = new char[image.Height, image.Width];

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

                return processedImage;
            }
        }

        public void ToImage(string filename)
        {
            Image<Rgba32> image = new Image<Rgba32>(this.width, this.height);
            ITopoArray<char> output = this.GetPropagator.ToValueArray<char>();

            for (var y = 0; y < this.width; y++)
            {
                for (var x = 0; x < this.width; x++)
                {
                    switch (output.Get(x, y))
                    {
                        case 'I':
                            image[x, y] = this.INDOOR_COLOR;
                            break;
                        case 'O':
                            image[x, y] = this.OUTDOOR_COLOR; //image[x, y] = random.Next(10) > 3 ? OUTDOOR_COLOR : WALL_COLOR;
                            break;
                        case 'X':
                            image[x, y] = this.WALL_COLOR;
                            break;
                        default:
                            break;
                    }
                }

            }
            image.SaveAsPng(filename);
        }
    }
}

