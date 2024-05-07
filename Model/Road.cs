using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityBuilder.Model
{
    public class Road : FieldData
    {
        //if we travelled through here when looking for connections
        private bool passed;
        //road connections data
        private int connections = 0;

        public Road(int x, int y, FieldType type) : base(x, y, type)
        {
            passed = false;
            ConnectsPolice = false;
        }


        public void Pass()
        {
            passed = true;
        }

        public void UnPass()
        {
            passed = false;
        }

        public bool IsPassed() { return passed; }

        public void MakeUndestroyable()
        {
            destroyable = false;
        }
        public int GetConnections()
        {
            return connections;
        }
        public void UpdateConnections(int v)
        {
            connections += v;
        }

        public void MakeDestroyable()
        {
            destroyable = true;
        }

        public bool ConnectsPolice { get; set; }

        public bool ConnectsStadium { get; set; }
    }
}
