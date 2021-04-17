using System;

using DeBroglie;
using DeBroglie.Topo;
using DeBroglie.Models;

using WFC.Utils;


namespace WFC
{
    public class Test
    {
        static void Main(string[] args)
        {
            ImageUtility.ProcessImage("./input.png");

            ITopoArray<char> sample = TopoArray.Create<char>(ImageUtility.GetProcessedImage, false);
            OverlappingModel model = new OverlappingModel(sample.ToTiles(), 3, 4, true);
            GridTopology topo = new GridTopology(90, 90, false);

            TilePropagator propagator = new TilePropagator(model, topo);

            Resolution status = propagator.Run();
            if (status != Resolution.Decided) throw new Exception("Undecided");

            ITopoArray<char> output = propagator.ToValueArray<char>();
            // Display the results
            for (var y = 0; y < 90; y++)
            {
                for (var x = 0; x < 90; x++)
                {
                    Console.Write(output.Get(x, y));
                }
                Console.WriteLine();
            }
        }
    }
}
