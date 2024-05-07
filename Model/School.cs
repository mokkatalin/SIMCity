using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityBuilder.Model
{
    /// <summary>
    /// School, people study here
    /// </summary>
    public class School : Educational
    {
        public School(int x, int y) : base(x, y, FieldType.School)
        {
        }
    }
}
