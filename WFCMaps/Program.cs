using System;
using CommandLine;

using WFC;
using WFC.Utils;


class Program
{
    static void Main(string[] args)
    {
        WaveFunctionCollapse wfc = new WaveFunctionCollapse("./input.png", 90, 90);

        wfc.CreateModel();
        wfc.RunModel();

        ExportUtility.ToImage("./output.png", wfc);
    }

}
