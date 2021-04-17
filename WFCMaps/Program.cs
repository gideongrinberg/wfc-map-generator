using System;
using System.Diagnostics;

using WFC;

public class Program
{
    static void Main()
    {
        WaveFunctionCollapse wfc = new WaveFunctionCollapse();
        wfc.Run();

        wfc.ToImage("./map.png");
        wfc.ToJson("./map.json", "testMap", true);
    }
}