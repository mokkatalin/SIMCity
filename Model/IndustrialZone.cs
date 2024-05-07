using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityBuilder.Model
{
    /// <summary>
    /// Industrial zone, people work here
    /// </summary>
    public class IndustrialZone : WorkPlace
    {
        public IndustrialZone(int x, int y) : base(x, y, FieldType.IndustrialZone)
        {
        }
    }
}
