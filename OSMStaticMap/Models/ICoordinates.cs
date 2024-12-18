using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMStaticMap.Models
{
    public interface ICoordinates
    {
        string Latitude { get; set; }
        string Longitude { get; set; }
        string GetAddress();
        CoordinatesModel GetCoordinates();
    }
}
