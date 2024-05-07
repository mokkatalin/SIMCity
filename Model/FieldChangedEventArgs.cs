using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityBuilder.Model
{
    /// <summary>
    /// Event argumentums for field update
    /// </summary>
    public class FieldChangedEventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public int Phase { get; set; }

        public FieldChangedEventArgs(int x, int y, int height, int width, int phase)
        {
            X = x;
            Y = y;
            Height = height;    
            Width = width;
            Phase = phase;
        }
    }
}