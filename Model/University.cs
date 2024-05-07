using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityBuilder.Model
{
    /// <summary>
    /// University, people study here
    /// </summary>
    public class University : Educational
    {
        public University(int x, int y) : base(x, y, FieldType.University)
        {
        }
    }
}
