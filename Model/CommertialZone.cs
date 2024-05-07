using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityBuilder.Model
{
    /// <summary>
    /// Commertial zone, people work here
    /// </summary>
    public class CommertialZone : WorkPlace
    {
        public CommertialZone(int x, int y) : base(x, y, FieldType.CommercialZone)
        {
        }
    }
}
