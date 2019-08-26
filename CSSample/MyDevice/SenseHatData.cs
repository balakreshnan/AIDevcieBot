using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDevice
{
    public class SenseHatData
    {
        public string deviceID { get; set; }
        public double? Humidity { get; set; }
        public double? Pressure { get; set; }
        public double? Temperature { get; set; }
        public string Location { get; set; }

        public Acceleration Acceleration { get; set; }
        public Gyro Gyro { get; set; }

        public Pose po { get; set; }

        public MagneticField mf { get; set; }

    }

    public class Acceleration
    {
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }

    }

    public class Gyro
    {
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }

    }

    public class Pose
    {
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }

    }

    public class MagneticField
    {
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }

    }

}
