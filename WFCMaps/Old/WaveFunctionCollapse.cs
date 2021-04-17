using System;
using System.IO;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using DeBroglie;
using DeBroglie.Topo;
using DeBroglie.Models;
using DeBroglie.Constraints;

using Newtonsoft.Json;

using WFC.Utils;

namespace WFC
{

    public class WaveFunctionCollapse
    {
        private int OUTPUT_WIDTH;
        private int OUTPUT_HEIGHT;

        private ITopoArray<char> sample;
        private ITopoArray<char> output;

        private OverlappingModel model;
        private GridTopology topology;
        private TilePropagator propagator;

        public readonly Rgba32 INDOOR_COLOR = new Rgba32(178, 178, 178, 255);
        public readonly Rgba32 OUTDOOR_COLOR = new Rgba32(255, 255, 255, 255);
        public readonly Rgba32 WALL_COLOR = new Rgba32(0, 0, 0, 255);

        private char[,] image;

        public TilePropagator GetPropagator { get => propagator; }
        public ITopoArray<char> GetOutput { get => output; set => output = value; }

        public int GetHeight { get => OUTPUT_HEIGHT; }
        public int GetWidth { get => OUTPUT_WIDTH; }
        /// <summary>
        /// Instantiates a new instance of <see cref="WaveFunctionCollapse"/>
        /// </summary>
        /// <param name="filename">The sample file</param>
        /// <param name="outputHeight">Height of the output in pixels</param>
        /// <param name="outputWidth">Width of the output in pixels</param>
        /// <param name="symetric">Is model symetric?</param>
        public WaveFunctionCollapse(string filename, int outputHeight, int outputWidth)
        {
            OUTPUT_HEIGHT = outputHeight;
            OUTPUT_WIDTH = outputWidth;

            ImageUtility.ProcessImage(filename);
            image = ImageUtility.GetProcessedImage;

            sample = TopoArray.Create(ImageUtility.GetProcessedImage, true);
        }

        public void CreateModel(int N = 4, int rotationalSym = 4, bool reflectionSym = true)
        {
            model = new OverlappingModel(sample.ToTiles(), N, rotationalSym, reflectionSym);

            topology = new GridTopology(OUTPUT_WIDTH, OUTPUT_HEIGHT, false);
            propagator = new TilePropagator(model, topology, backtrack: true, constraints: new ITileConstraint[] { new MirrorXConstraint() });

            Console.WriteLine($"Successfully created model. NxN is {model.NX}x{model.NY}.");
        }

        /// <summary>
        /// Runs the wave function collapse algorithim. Must be run after <see cref="CreateModel(int, int, bool, bool)"/>
        /// </summary>
        public void RunModel()
        {
            Console.WriteLine("Running Model");
            Resolution status = propagator.Run();

            if (status != Resolution.Decided) throw new Exception("Propagator status was undecided");
            if (status == Resolution.Decided) Console.WriteLine($"Finished solving wave function.");
        }


        /// <summary>
        /// Save converts the WFC's output (a 2D array of <see cref="char"/>s) into a PNG and save it to a file.
        /// </summary>
        /// <param name="filename">The file to save to.</param>
        /// <param name="useUniqueName">Should a unique identifer be added to <paramref name="filename"/>. If <paramref name="useUniqueName"/> is true, the filename shouldn't have an extension./></param>
        public void ToImage(string filename, bool useUniqueName = true)
        {
            filename = useUniqueName ? ExportUtility.GenerateFileName(filename) : filename;

            Image<Rgba32> image = new Image<Rgba32>(OUTPUT_WIDTH, OUTPUT_HEIGHT);
            output = propagator.ToValueArray<char>();
            for (var y = 0; y < OUTPUT_WIDTH; y++)
            {
                for (var x = 0; x < OUTPUT_HEIGHT; x++)
                {
                    switch (output.Get(x, y))
                    {
                        case 'I':
                            image[x, y] = INDOOR_COLOR;
                            break;
                        case 'O':
                            image[x, y] = OUTDOOR_COLOR; //image[x, y] = random.Next(10) > 3 ? OUTDOOR_COLOR : WALL_COLOR;
                            break;
                        case 'X':
                            image[x, y] = WALL_COLOR;
                            break;
                        default:
                            break;
                    }
                }
            }

            image.SaveAsPng(filename);
            Console.WriteLine($"Wrote image output to {filename}");
            //Process.Start("/usr/bin/open", filename);
        }

        public string ToJson(bool prettyPrint = false, string mapName = "newMap")
        {
            return JsonConvert.SerializeObject(new {
                    mapName = mapName,
                    height = OUTPUT_HEIGHT,
                    width = OUTPUT_WIDTH,
                    map = propagator.ToValueArray<char>().ToArray2d<char>()},
                    formatting: prettyPrint ? Formatting.Indented : Formatting.None
                );
        }

#nullable enable
        public void ToJson(string filename = "output.json", bool prettyPrint = true, string? mapName = null)
        {
            string json;
            if (mapName != null)
            {
                json = this.ToJson(prettyPrint, mapName);
            }
            else if (filename != "output.json" && mapName == null)
            {
                json = ToJson(prettyPrint, filename.Substring(0, filename.LastIndexOf(".")));
            }
            else if (mapName != null)
            {
                json = ToJson(prettyPrint, mapName);
            }
            else
            {
                json = ToJson(prettyPrint);
            }

            File.WriteAllText(filename, json);
        }
#nullable disable

        /// <returns>A <see cref="string"/> representation of the <see cref="WaveFunctionCollapse"/>'s output.</returns>
        public override string ToString()
        {
            string retVal = "";
            output = propagator.ToValueArray<char>();
            for (var y = 0; y < OUTPUT_WIDTH; y++)
            {
                for (var x = 0; x < OUTPUT_HEIGHT; x++)
                {
                    retVal += output.Get(x, y);
                }
                retVal += "\n";
            }

            return retVal;
        }

    }
}

