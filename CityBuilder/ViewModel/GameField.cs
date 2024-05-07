using CityBuilder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CityBuilder.ViewModel
{
    /// <summary>
    /// Class for the buttons from the game area
    /// </summary>
    public class GameField : ViewModelBase
    {
        private int _capacity;
        private int _occupancy;
        private FieldType _viewFieldType;
        private string _imagePath = "";


        public int X { get; set; }

        public int Y { get; set; }

        public int Number { get; set; }

        public int Capacity { get { return _capacity; }
            set { 
                if(_capacity != value)
                {
                    _capacity = value;
                    OnPropertyChanged();
                }
            } 
        }

        public int Occupancy
        {
            get { return _occupancy; }
            set
            {
                if (_occupancy != value)
                {
                    _occupancy = value;
                    OnPropertyChanged();
                }
            }
        }

        public FieldType ViewFieldType { get { return _viewFieldType; }
            set {
                if (_viewFieldType != value)
                {
                    _viewFieldType = value;
                    OnPropertyChanged();
                }
            } 
        }

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




        public DelegateCommand? SelectMapCommand { get; set; }

    }
}
