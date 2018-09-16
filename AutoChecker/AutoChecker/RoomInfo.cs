using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoChecker
{
    class RoomInfo
    {
        public string Name { get; private set; } = string.Empty;
        public string Location { get; private set; } = string.Empty;
        public string Img { get; private set; } = string.Empty;
        public double Area { get; private set; } = 0;
        public string Floor { get; private set; } = string.Empty;
        public string Structure { get; private set; } = string.Empty;
        public List<string> Styles { get; private set; } = null;

        public RoomInfo(
            string name,
            string location,
            string img,
            double area,
            string floor,
            string structure,
            List<string> styles)
        {
            Name = name;
            Location = location;
            Img = img;
            Area = area;
            Floor = floor;
            Structure = structure;
            Styles = styles;
        }
    }
}
