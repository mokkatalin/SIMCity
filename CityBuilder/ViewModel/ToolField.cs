using CityBuilder.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CityBuilder.ViewModel
{
    /// <summary>
    /// Class for the buttons from the tool area
    /// </summary>
    public class ToolField : ViewModelBase
    {
        private FieldType _toolFieldType;

        private string _imagePath = "";

        public int X { get; set; }

        public int Y { get; set; }

        public int Number { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public FieldType ToolFieldType
        {
            get { return _toolFieldType; }
            set
            {
                if (_toolFieldType != value)
                {
                    _toolFieldType = value;
                    OnPropertyChanged();
                }
            }
            
        }

        public DelegateCommand? SelectToolCommand { get; set; }

        public String ImagePath
        {
            get { return _imagePath; }
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
