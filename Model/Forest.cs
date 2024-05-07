using CityBuilder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityBuilder.Model
{
    public class Forest : FieldData
    {
        private int age;
        private Boolean isGrown;
        public Forest(int x, int y, FieldType type) : base(x, y, type)
        {
            age = 0;
            isGrown = false;
            destroyable = false;
        }

        public void Grow()
        {
            age++;
            if (age >= 10) isGrown = true;
        }

        public Boolean IsGrown()
        {
            return isGrown;
        }

        public int GetAge()
        {
            return age;
        }

        public override String GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Age: " + age  + " years old\n");
            String growth = isGrown ? "Already grown.\n" : "Is not grown yet.\n";
            sb.Append(growth);
            return sb.ToString();
        }

    }
}
