using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    enum Colour
    {
        Green = 0,
        Amber,
        Red,
        None
    }

    enum RainTrend
    { 
        Increasing = 0,
        Decreasing
    }
    internal class Device
    {
        public int DeviceID { get; set; }
        public string? DeviceName { get; set; }
        public string? Location { get; set; }
        public int AverageRainFall { get; set; }
        public Colour Density { get; set; }
        //public RainTrend Trend { get; set; }

    }
}
