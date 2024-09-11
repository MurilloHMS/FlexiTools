using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexiTools.MVVM.Model
{
    internal class Vehicle
    {
        public string Name { get; set; }
        public string Plate { get; set; }
        public string Brand { get; set; }
        public double UrbanAlcoholConsumption { get; set; }
        public double RoadAlcoholConsuption { get; set; }
        public double UrbanGasolineConsumption { get; set; }
        public double RoadGasolineConsuption { get; set; }
    }
}
