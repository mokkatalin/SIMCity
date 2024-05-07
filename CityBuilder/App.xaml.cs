using CityBuilder.Model;
using CityBuilder.View;
using CityBuilder.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CityBuilder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private CityModel _model = null!;
        private CityViewModel _viewModel = null!;
        private MainMenu _mainMenu = null!;
        private StartWindow _startWindow = null!;
        private MainWindow _mainWindow = null!;
        private HelpWindow _helpWindow = null!;
        private StatWindow _statWindow = null!;
        private GameOverWindow _gameOverWindow = null!;
        private DispatcherTimer _timer = null!;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _model = new CityModel();
            _model.GameOver += new EventHandler(Model_GameOver);

            _viewModel = new CityViewModel(_model);
            _viewModel.NewGame += new EventHandler(ViewModel_NewGame);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            _viewModel.SpeedChange += new EventHandler(ViewModel_SpeedChange);
            _viewModel.OpenStartWindowEvent += new EventHandler(OpenStartWindowEventHandler);
            _viewModel.OpenMainMenuEvent += new EventHandler(OpenMainMenuEventHandler);
            _viewModel.OpenHelpWindowEvent += new EventHandler(OpenHelpWindowEventHandler);
            _viewModel.OpenStatWindowEvent += new EventHandler(OpenStatWindowEventHandler);

            _mainMenu = new MainMenu();
            _mainMenu.DataContext = _viewModel;
            _mainMenu.Show();
            

            //Lehet ez mashova kerul
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.2);
            _timer.Tick += new EventHandler(Timer_Tick);
            
        }

        private void KeyPressed(object? sender, KeyEventArgs e)
        {
            if(e.Key == Key.P)
            {
                if (_timer.IsEnabled)
                {
                    _timer.Stop();
                    _viewModel.GameOn = false;
                }
                else
                {
                    _timer.Start();
                    _viewModel.GameOn = true;
                }
            }
        }

        private void OpenStatWindowEventHandler(object? sender, EventArgs e)
        {
            _statWindow = new StatWindow();
            _statWindow.DataContext = _viewModel;
            _statWindow.ShowDialog();

        }

        private void OpenHelpWindowEventHandler(object? sender, EventArgs e)
        {
            _helpWindow = new HelpWindow();
            _helpWindow.ShowDialog();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _model.TimeAdvanced();
        }

        private void ViewModel_NewGame(object? sender, EventArgs e)
        {
            _model.CreateMap(_viewModel.Input);
            _timer.Start();
            _mainWindow = new MainWindow();
            _mainWindow.DataContext = _viewModel;
            _mainWindow.KeyDown += new KeyEventHandler(KeyPressed);
            _mainWindow.Show();
            _startWindow.Close();
        }

        private void ViewModel_SpeedChange(object? sender, EventArgs e)
        {
            double newInterval = 0;
            switch (_viewModel.Speed)
            {
                case 1:
                    newInterval = 0.5;
                    break;
                case 2:
                    newInterval = 0.2;
                    break;
                case 3:
                    newInterval = 0.05;
                    break;
            }

            _timer.Interval = TimeSpan.FromSeconds(newInterval);
        }
        private void OpenMainMenuEventHandler(object? sender, EventArgs e)
        {
            _mainMenu = new MainMenu();
            _mainMenu.DataContext = _viewModel;
            _mainMenu.Show();
            _startWindow?.Close();
            _mainWindow?.Close();
            _gameOverWindow?.Close();
        }

        private void ViewModel_ExitGame(object? sender, EventArgs e)
        {
            _mainMenu?.Close();
            _mainWindow?.Close();
        }

        private void OpenStartWindowEventHandler(object? sender, EventArgs e)
        {
            _startWindow = new StartWindow();
            _startWindow.DataContext = _viewModel;
            _startWindow.Show();
            _mainMenu.Close();           
        }

        private void Model_GameOver(object? sender, EventArgs e)
        {
            _timer.Stop();
            _gameOverWindow = new GameOverWindow();
            _gameOverWindow.DataContext = _viewModel;
            _gameOverWindow.ShowDialog();
        }
    }
}
