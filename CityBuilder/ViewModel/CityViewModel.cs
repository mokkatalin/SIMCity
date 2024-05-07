using CityBuilder.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace CityBuilder.ViewModel
{
    public class CityViewModel : ViewModelBase
    {
        #region Private data

        private CityModel _model;

        private Boolean _status;
        private int _speed;
        private int _selectedType = -1;
        private DateTime date;
        private FieldData? _selectedField = null;
        private Visibility _upgradeButtonVisibility = Visibility.Hidden;
        #endregion

        #region Events

        public event EventHandler? NewGame;

        public event EventHandler? ExitGame;
        
        public event EventHandler? SpeedChange;
        
        public event EventHandler? StopResume;
        
        public event EventHandler? OpenStartWindowEvent;

        public event EventHandler? OpenMainMenuEvent;

        public event EventHandler? OpenHelpWindowEvent;

        public event EventHandler? OpenStatWindowEvent;

        #endregion

        #region Properties, Getters
        /// <summary>
        /// Getter for the elapsed time
        /// </summary>
        public string FinalTime {
            get {
                TimeSpan timeDifference = Date - DateTime.Now;
                int years = (int)timeDifference.TotalDays / 365;
                int months = (int)((timeDifference.TotalDays % 365) / 30.436875);//30.436875 is the approximate number of days in a month
                int days = (int)(timeDifference.TotalDays % 30.436875);
                if (years == 0)
                {
                    return $"{months} months, {days} days";
                }
                else if(years == 1)
                {
                    return $"{years} year, {months} months, {days} days";
                }
                else
                {
                    return $"{years} years, {months} months, {days} days";
                }
            } 
        }
        /// <summary>
        /// Getter for the actual date, string formatted
        /// </summary>
        public string Time { get { return Date.ToString("yyyy-MM-dd"); } }
        /// <summary>
        /// Getter for the date
        /// </summary>
        public DateTime Date { get { return date; } }
        /// <summary>
        /// Getter for the balance
        /// </summary>
        public int Balance { get { return _model.GetBalance; } }
        /// <summary>
        /// Getter for the happiness formatted to percentage
        /// </summary>
        public string Happiness { get { return (_model.GetHappiness / 5).ToString() + "%"; } }
        /// <summary>
        /// Getter for the happiness for the progressbar
        /// </summary>
        public int ProgressBarHappiness
        {
            get
            {
                return _model.GetHappiness / 5;
            }
        }
        /// <summary>
        /// Getter for the number of people who finished school
        /// </summary>
        public string SchoolFinished { get { return _model.GetSchoolDiploma.ToString(); } }
        /// <summary>
        /// Getter for the number of people who finished university
        /// </summary>
        public string UniFinished { get { return _model.GetUniversityDiploma.ToString(); } }
        /// <summary>
        /// Getter and setter for the current status of the game
        /// </summary>
        public bool GameOn
        {
            get { return _status; } 
            set { 
                if(_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                } 
            } 
        }
        /// <summary>
        /// Getter for the city population
        /// </summary>
        public int Population { get { return _model.GetPopulation; } }
        /// <summary>
        /// Getter for the current status of the game, string formatted
        /// </summary>
        public string Status 
        { 
            get 
            { if (_status)
                {
                    return "Unpaused";
                }
                else
                {
                    
                    return "Paused";
                }
            } 
        }
        /// <summary>
        /// Getter for the city name
        /// </summary>
        public string Name { get { return _model.GetCityName; } }
        /// <summary>
        /// Getter for the game speed
        /// </summary>
        public int Speed { get { return _speed; } } //

        //Textbox from view
        /// <summary>
        /// Getter and setter for the inputbox binding from StartWindow
        /// </summary>
        public string Input { get; set; }
        
        /// <summary>
        /// Getter for the zonedata, when clicked with cursor tool
        /// </summary>
        public String ZoneData
        {            
            get
            {
                if (_selectedType == -1)
                {
                    if (_selectedField != null)
                    {
                        if(_selectedField is Zone z)
                        {
                            if(z.GetCurrentLevel == 3)
                            {
                                UpgradeButtonVisibility = Visibility.Hidden;
                            }
                            else
                            {
                                UpgradeButtonVisibility = Visibility.Visible;
                            }                           
                        }
                        else
                        {
                            UpgradeButtonVisibility = Visibility.Hidden;
                        }
                        return _selectedField.GetInfo();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    UpgradeButtonVisibility = Visibility.Hidden;
                    return "";
                }
            }
        }
        /// <summary>
        /// Getter and setter for the upgrade button visibility binding
        /// </summary>
        public Visibility UpgradeButtonVisibility
        {
            get { return _upgradeButtonVisibility; }
            set { _upgradeButtonVisibility = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// Getter and setter for the taxes
        /// </summary>
        public int Tax { 
            get { return _model.GetTax; }
            set
            {
                if (_model.GetTax != value)
                {
                    _model.SetTax(value);
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Getter and setter for the slow speed option
        /// </summary>
        public bool SlowSpeed
        {
            get { return _speed == 1; }
            set
            {
                if (_speed == 1)
                {
                    return;
                }
                _speed = 1;
                OnPropertyChanged(nameof(SlowSpeed));
                OnPropertyChanged(nameof(NormalSpeed));
                OnPropertyChanged(nameof(FastSpeed));
            }
        }
        /// <summary>
        /// Getter and setter for the normal speed option
        /// </summary>
        public bool NormalSpeed
        {
            get { return _speed == 2; }
            set
            {
                if (_speed == 2)
                {
                    return;
                }
                _speed = 2;
                OnPropertyChanged(nameof(SlowSpeed));
                OnPropertyChanged(nameof(NormalSpeed));
                OnPropertyChanged(nameof(FastSpeed));
            }
        }
        /// <summary>
        /// Getter and setter for the fast speed option
        /// </summary>
        public bool FastSpeed
        {
            get { return _speed == 4; }
            set
            {
                if (_speed == 4)
                {
                    return;
                }
                _speed = 4;
                OnPropertyChanged(nameof(SlowSpeed));
                OnPropertyChanged(nameof(NormalSpeed));
                OnPropertyChanged(nameof(FastSpeed));
            }
        }
        /// <summary>
        /// The ObservableCollection where the GameField based buttons are stored
        /// </summary>
        public ObservableCollection<GameField> Fields { get; set; }
        /// <summary>
        /// The ObservableCollection where the ToolField based buttons are stored
        /// </summary>
        public ObservableCollection<ToolField> ToolFields { get; set; }
        /// <summary>
        /// The ObservableCollection where the last 5 income texts are stored
        /// </summary>
        public ObservableCollection<Statistics> Incomes { get; set; }
        /// <summary>
        /// The ObservableCollection where the last 5 expense texts are stored
        /// </summary>
        public ObservableCollection<Statistics> Expenses { get; set; }
        /// <summary>
        /// Getter and setter for the currently selected tool
        /// </summary>
        public int SelectedType { get { return _selectedType; }
            set
            {
                if(_selectedType != value)
                {
                    _selectedType = value;
                    OnPropertyChanged();
                }
            }
        }

        public DelegateCommand NewGameCommand { get; private set; }

        public DelegateCommand ExitGameCommand { get; private set; }

        public DelegateCommand SlowSpeedCommand { get; private set; }

        public DelegateCommand NormalSpeedCommand { get; private set; }
    
        public DelegateCommand FastSpeedCommand { get; private set; }
        public DelegateCommand StopResumeCommand { get; private set; }

        public DelegateCommand OpenStartWindowCommand { get; private set; }

        public DelegateCommand OpenMainMenuCommand { get; private set; }

        public DelegateCommand OpenHelpWindowCommand { get; private set; }

        public DelegateCommand OpenStatWindowCommand { get; private set; }

        public DelegateCommand UpgradeCommand { get; private set; }

        public DelegateCommand CatastropheCommand { get; private set; }
        #endregion

        #region Constructor

        /// <summary>
        /// ViewModel constructor based on model
        /// </summary>
        /// <param name="model">Model</param>
        public CityViewModel(CityModel model)
        {
            _model = model;
            _model.GameAdvanced += new EventHandler(Model_GameAdvanced);
            //_model.GameOver += new EventHandler(Model_GameOver);
            _model.GameCreated += new EventHandler(Model_GameCreated); //new
            _model.IncomeChanged += new EventHandler(Model_IncomeChanged);
            _model.ExpenseChanged += new EventHandler(Model_ExpenseChanged);
            _model.FieldChanged += new EventHandler<FieldChangedEventArgs>(OnFieldChanged);
            _model.CatastropheChanged += new EventHandler(Model_CatastropheChanged);

            date = DateTime.Now;
            _speed = 2;
            _status = true;
            OnPropertyChanged();
            NewGameCommand = new DelegateCommand(param => OnNewGame());
            ExitGameCommand = new DelegateCommand(param => OnExitGame());

            CatastropheCommand = new DelegateCommand(param => OnManualCatastrophe());

            SlowSpeedCommand = new DelegateCommand(param => OnSpeedChange(Convert.ToInt32(1)));
            NormalSpeedCommand = new DelegateCommand(param => OnSpeedChange(Convert.ToInt32(2)));
            FastSpeedCommand = new DelegateCommand(param => OnSpeedChange(Convert.ToInt32(3)));
            
            OpenStartWindowCommand = new DelegateCommand(param => OpenStartWindowEvent?.Invoke(this, EventArgs.Empty));
            OpenMainMenuCommand = new DelegateCommand(param => OpenMainMenuEvent?.Invoke(this, EventArgs.Empty));
            OpenHelpWindowCommand = new DelegateCommand(param => OpenHelpWindowEvent?.Invoke(this, EventArgs.Empty));
            OpenStatWindowCommand = new DelegateCommand(param => OpenStatWindowEvent?.Invoke(this, EventArgs.Empty));
            
            UpgradeCommand = new DelegateCommand(param => UpgradeZone());

        }
        private void RefreshGrid()
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    Fields[i * 100 + j].ImagePath = GetImagePath(i, j, _model.GetField(i, j).GetX, _model.GetField(i, j).GetY, GetPhase(_model.GetField(i, j)));                   
                }
            }
            OnPropertyChanged(nameof(Fields));
        }

        private int GetPhase(FieldData field)
        {
            if (field is Zone zone)
            {
                if (zone.GetOccupancy > 0)
                {
                    if (zone.GetOccupancy <= 5)
                    {
                        return 1;
                    }
                    else if (zone.GetOccupancy <= 10)
                    {
                        return 2;
                    }
                    else if (zone.GetOccupancy <= 20)
                    {
                        return 3;
                    }
                }
            }
            return 0;
        }
        private string GetImagePath(int i, int j, int x, int y, int phase)
        {
            string path = "/CityBuilder;component/View/images/";
            FieldData field = _model.GetField(x, y);
            if (field.IsDemolished)
            {
                Random r = new Random();
                path += "demo" + r.Next(0, 9) + ".png";
            }
            else
            {
                switch (field.GetType)
                {
                    case FieldType.Empty:
                        path += "empty.png";
                        break;
                    case FieldType.Road:
                        int num = ChangeNum(((Road)field).GetConnections());
                        path += "road" + num + ".png";
                        break;
                    case FieldType.CommercialZone:
                        if (((Zone)field).GetCurrentLevel == 1)
                        {
                            if (phase == 0)
                            {
                                path += "comm" + (i - x) + (j - y) + ".png";
                            }
                            else
                            {
                                path += "comm" + (i - x) + (j - y) + phase + ".png";
                            }
                        }
                        else if (((Zone)field).GetCurrentLevel == 2)
                        {
                            path += "comm" + (i - x) + (j - y) + "4.png";
                        }
                        else if (((Zone)field).GetCurrentLevel == 3)
                        {
                            path += "comm" + (i - x) + (j - y) + "5.png";
                        }
                        break;
                    case FieldType.ResidentialZone:
                        if (((Zone)field).GetCurrentLevel == 1)
                        {
                            if (phase == 0)
                            {
                                path += "res" + (i - x) + (j - y) + ".png";
                            }
                            else
                            {
                                path += "res" + (i - x) + (j - y) + phase + ".png";
                            }
                        }
                        else if (((Zone)field).GetCurrentLevel == 2)
                        {
                            path += "res" + (i - x) + (j - y) + "4.png";
                        }
                        else if (((Zone)field).GetCurrentLevel == 3)
                        {
                            path += "res" + (i - x) + (j - y) + "5.png";
                        }
                        break;
                    case FieldType.IndustrialZone:
                        if (((Zone)field).GetCurrentLevel == 1)
                        {
                            if (phase == 0)
                            {
                                path += "ind" + (i - x) + (j - y) + ".png";
                            }
                            else
                            {
                                path += "ind" + (i - x) + (j - y) + phase + ".png";
                            }
                        }
                        else if (((Zone)field).GetCurrentLevel == 2)
                        {
                            path += "ind" + (i - x) + (j - y) + "4.png";
                        }
                        else if (((Zone)field).GetCurrentLevel == 3)
                        {
                            path += "ind" + (i - x) + (j - y) + "5.png";
                        }
                        break;
                    case FieldType.Stadium:
                        path += "stadium" + (i - x) + (j - y) + ".png";
                        break;
                    case FieldType.University:
                        path += "uni" + (i - x) + (j - y) + ".png";
                        break;
                    case FieldType.Police:
                        path += "police" + (i - x) + (j - y) + ".png";
                        break;
                    case FieldType.School:
                        path += "school" + (i - x) + (j - y) + ".png";
                        break;
                    case FieldType.Forest:
                        int forestAge = ((Forest)field).GetAge();
                        if (forestAge < 2)
                        {
                            path += "forest1.png";
                        }
                        else if(forestAge < 5)
                        {
                            path += "forest2.png";
                        }
                        else if(forestAge < 10)
                        {
                           path += "forest3.png";        
                        }
                        else
                        {
                            path += "forest4.png";
                        }
                        break;
                }
            }           
            return path;
        }

        private int ChangeNum(int num)
        {
            if (num == 0 || num == 2 || num == 8)
            {
                return 10;
            }
            else if (num == 1 || num == 4)
            {
                return 5;
            }
            return num;
        }


        private void OnManualCatastrophe()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            _model.Catastrophe(r.Next(100), r.Next(100));
        }

        private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
        {    
            for (int i = e.X; i < e.X + e.Height; i++)
            {
                for (int j = e.Y; j < e.Y + e.Width; j++)
                {
                    if (i <= 100 && j < 100)
                    {
                        Fields[i * 100 + j].ImagePath = GetImagePath(i, j, e.X, e.Y, e.Phase);
                    }
                }
            }
            OnPropertyChanged(nameof(Fields));
            OnPropertyChanged(nameof(Balance));
        }

        #endregion

        #region Private methods

        private void Model_IncomeChanged(object o, EventArgs e)
        {
            Incomes = new ObservableCollection<Statistics>();
            foreach(String income in _model.GetIncomes)
            {
                Incomes.Add(new Statistics { Line = income });
            }
            OnPropertyChanged(nameof(Incomes));
        }

        private void Model_ExpenseChanged(object o, EventArgs e)
        {
            Expenses = new ObservableCollection<Statistics>();
            foreach (String expense in _model.GetExpenses)
            {
                Expenses.Add(new Statistics{Line = expense});
            }
            OnPropertyChanged(nameof(Expenses));
        }

        
        private void Model_GameAdvanced(object o, EventArgs e)
        {
            date = date.AddDays(1);
            UpdateZoneData();
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Time));
            OnPropertyChanged(nameof(Balance));
            OnPropertyChanged(nameof(Happiness));
            OnPropertyChanged(nameof(ProgressBarHappiness));
            OnPropertyChanged(nameof(Population));
            OnPropertyChanged(nameof(SchoolFinished));
            OnPropertyChanged(nameof(UniFinished));
        }

        private void Model_GameCreated(object o, EventArgs e)
        {
            Fields = new ObservableCollection<GameField>();
            for (Int32 i = 0; i < 100; i++)
            {
                for (Int32 j = 0; j < 100; j++)
                {
                    Fields.Add(new GameField
                    {
                        X = i,
                        Y = j,
                        Number = i * 100 + j,
                        ImagePath = GetImagePath(i, j, i, j, 0),
                        SelectMapCommand = new DelegateCommand(param => SelectMap(Convert.ToInt32(param)))
                    });
                }
            }

            ToolFields = new ObservableCollection<ToolField>();


            ToolFields.Add(new ToolField{ X = 0, Y = 0, ImagePath = "/CityBuilder;component/View/images/tool_cursor.png",Title = "Information",Description = "Click on a field to get some information.", SelectToolCommand = new DelegateCommand(param => SelectTool(-1)) });
            ToolFields.Add(new ToolField { X = 0, Y = 1, ImagePath = "/CityBuilder;component/View/images/tool_destroy.png",Title = "Unbuild" ,Description = "Destroy a field", SelectToolCommand = new DelegateCommand(param => SelectTool(0)) });
            ToolFields.Add(new ToolField { X = 1, Y = 0, ImagePath = "/CityBuilder;component/View/images/road10.png",Title = "Road", Description = "Size: 1x1\nCost: 200", SelectToolCommand = new DelegateCommand(param => SelectTool(1)) });
            ToolFields.Add(new ToolField { X = 1, Y = 1, ImagePath = "/CityBuilder;component/View/images/res11.png",Title = "Residental zone", Description = "Size: 3x3\nCost: 1000", SelectToolCommand = new DelegateCommand(param => SelectTool(2)) });
            ToolFields.Add(new ToolField { X = 2, Y = 0, ImagePath = "/CityBuilder;component/View/images/ind11.png", Title = "Industrial zone", Description = "Size: 3x3\nCost: 1000", SelectToolCommand = new DelegateCommand(param => SelectTool(3)) });
            ToolFields.Add(new ToolField { X = 2, Y = 1, ImagePath = "/CityBuilder;component/View/images/comm11.png", Title = "Commertial zone", Description = "Size: 3x3\nCost: 1000", SelectToolCommand = new DelegateCommand(param => SelectTool(4)) });
            ToolFields.Add(new ToolField { X = 3, Y = 0, ImagePath = "/CityBuilder;component/View/images/tool_police.png", Title = "Police station", Description = "Size: 2x2\nCost: 1500", SelectToolCommand = new DelegateCommand(param => SelectTool(5)) });
            ToolFields.Add(new ToolField { X = 3, Y = 1, ImagePath = "/CityBuilder;component/View/images/tool_stadium.png", Title = "Stadium", Description = "Size: 2x2\nCost: 1500" ,SelectToolCommand = new DelegateCommand(param => SelectTool(6)) });
            ToolFields.Add(new ToolField { X = 4, Y = 0, ImagePath = "/CityBuilder;component/View/images/tool_school.png", Title = "School", Description = "Size: 2x1\nCost: 1500" ,SelectToolCommand = new DelegateCommand(param => SelectTool(7)) });
            ToolFields.Add(new ToolField { X = 4, Y = 1, ImagePath = "/CityBuilder;component/View/images/tool_uni.png", Title = "University", Description = "Size: 2x2\nCost: 1500" ,SelectToolCommand = new DelegateCommand(param => SelectTool(8)) });
            ToolFields.Add(new ToolField { X = 5, Y = 0, ImagePath = "/CityBuilder;component/View/images/forest1.png", Title = "Forest", Description = "Size: 1x1\nCost: 2000", SelectToolCommand = new DelegateCommand(param => SelectTool(9)) });


            OnPropertyChanged(nameof(Fields));
            OnPropertyChanged(nameof(ToolFields));


            //refresh table()
        }

        private void Model_CatastropheChanged(object o, EventArgs e)
        {
            RefreshGrid();
        }

        private void SelectMap(int number)
        {
            try
            {
                int i = number / 100;
                int j = number % 100;
                
                if (SelectedType == -1)
                {                   
                    _selectedField = _model.GetField(i, j);
                    OnPropertyChanged(nameof(ZoneData));
                } else if (SelectedType == 0)
                {
                    _model.UnBuild(i, j);                   
                } else
                {
                    UpgradeButtonVisibility = Visibility.Hidden;
                    _model.Build(i, j, (FieldType)SelectedType);
                }
                
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("You can not build to the edge of the map!", "Building error", MessageBoxButton.OK, MessageBoxImage.Error);
            }        
            catch (Exception e)
            {
                if (e.Message.ToString().Contains("destroy"))
                {
                    MessageBox.Show(e.Message, "Unbuilding error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(e.Message, "Building error", MessageBoxButton.OK, MessageBoxImage.Error);
                }              
            }
            
        }

        private void UpdateZoneData()
        {
            if(_selectedField != null)
            {
                OnPropertyChanged(nameof(ZoneData));
            }
        }
        
        private void UpgradeZone()
        {
            if(_selectedField != null && _selectedField is Zone)
            {
                _model.Upgrade((Zone)_selectedField);
                OnPropertyChanged(nameof(ZoneData));
            }
        }
        private void SelectTool(int number)
        {
            SelectedType = number;
        }

        #endregion

        #region Event triggers

        private void OnNewGame()
        {
            //Feltetelt majd valszeg cserelni kell
            if (Input != "" && Input is not null)
            {
                NewGame?.Invoke(this, EventArgs.Empty);
            } else
            {
                Input = "";
                OnPropertyChanged(nameof(Input));
            }
        }

        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnStopResume()
        {
            StopResume?.Invoke(this, EventArgs.Empty);
        }

        private void OnSpeedChange(int s)
        {
            _speed = s;
            SpeedChange?.Invoke(this, EventArgs.Empty);
        }


        #endregion

    }
}
