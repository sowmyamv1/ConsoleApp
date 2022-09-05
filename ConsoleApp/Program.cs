using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

namespace ConsoleApp
{
    internal class Program
    {
        public static List<DeviceReadings> Readings = new List<DeviceReadings>();
        public static List<Device> Devices = new List<Device>();

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");

            //Fetch the Devices file 
            var curDir = Directory.GetCurrentDirectory();
            //Console.WriteLine(curDir);
            var PathtoDevicesFile = Path.GetFullPath(Path.Combine(curDir, @"..\..\..\..\Devices.csv"));

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                IgnoreBlankLines = false,
            };

            using (var reader = new StreamReader(PathtoDevicesFile))
            //using (var csv = new CsvReader((IParser)reader))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    if (string.IsNullOrEmpty(csv.GetField(0)))
                        break;
                    
                    var Record = new Device
                    {
                        DeviceID = csv.GetField<int>("Device ID"),
                        DeviceName = csv.GetField<string>("Device Name"),
                        Location = csv.GetField<string>("Location")
                    };
                    Devices.Add(Record);
                }
            }

            //Read Data 1 file & Data 2 file
            //Iterate through the collection for each Device Id.
            //Check time stamp to be within last 4 hrs, if beyond 4 hrs, ignore the iteration
            //Add the Rainfall value & calculate Average. Once average calculated, determine Density
            //Determine the Rain Trend based the average values of all devices.

            var PathtoDataFile1 = Path.GetFullPath(Path.Combine(curDir, @"..\..\..\..\Data1.csv"));
            var PathtoDataFile2 = Path.GetFullPath(Path.Combine(curDir, @"..\..\..\..\Data2.csv"));

            ReadDeviceReadings(PathtoDataFile1);
            ReadDeviceReadings(PathtoDataFile2);

            //Calculate the Average, Density & Rain Trend
            foreach( var device in Devices)
            {
                int Sum = 0, Count = 0;
                device.Density = Colour.None; //Set default

                foreach (var Reading in Readings)
                {
                    if (device.DeviceID == Reading.DeviceID)
                    {
                        DateTime CurrentTime = DateTime.Now;
                        DateTime RecordedTime = Reading.Time;
                        int TimeDiffHours = CurrentTime.Subtract(RecordedTime).Hours;
                        //if(TimeDiffHours > 4)
                        //    continue;
                        Sum += Reading.Rainfall;
                        Count++;
                        if (Reading.Rainfall > 30)
                            device.Density = Colour.Red;
                    }
                }
                
                device.AverageRainFall = Sum /Count;

                if(device.Density == Colour.None)
                {
                    if (device.AverageRainFall < 10)
                        device.Density = Colour.Green;
                    else if (device.AverageRainFall < 15)
                        device.Density = Colour.Amber;
                    else
                        device.Density = Colour.Red;
                }
            }
            DisplayAverageData();
        }

        public static void ReadDeviceReadings(string path)
        {
            //Iterate through the collection for each Device Id.
            //Check time stamp to be within last 4 hrs, if beyond 4 hrs, ignore the iteration
            //Add the Rainfall value & calculate Average. Once average calculated, determine density
            //Determine the Rain Trend based the average values of all devices.

            //Read Data1 CSV file
            using (var reader = new StreamReader(path))
            //using (var csv = new CsvReader((IParser)reader))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var Record = new DeviceReadings
                    {
                        DeviceID = csv.GetField<int>("Device ID"),
                        Time = csv.GetField<DateTime>("Time"),
                        Rainfall = csv.GetField<int>("Rainfall")
                    };
                    Readings.Add(Record);
                }
            }
        }

        public static void DisplayAverageData()
        {
            Console.WriteLine("The list shows the Average Device Readings from the Device\n");
            Console.WriteLine("Device ID    |   Device Name     |       Location    |   Average RainFall    |   Density\n");
            foreach (var device in Devices)
            {
                Console.WriteLine("{0, 10}{1, 15}{2, 25}{3,15}{4,20}\n", device.DeviceID, device.DeviceName, device.Location, device.AverageRainFall, device.Density);
                //Console.WriteLine($"{device.DeviceID}\t\t{device.DeviceName}\t\t{device.Location}\t\t{device.AverageRainFall}\t\t{device.Density}\n");
                //Console.WriteLine($"Device ID : {device.DeviceID}\nDevice Name : {device.DeviceName}\nLocation : {device.Location}\nAverage RainFall : {device.AverageRainFall}\nDensity : {device.Density}\n");

            }
        }
    }
}