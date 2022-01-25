using KEKWSoundboard.Components;
using KEKWSoundboard.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KEKWSoundboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int? _currentFolderId;

        int _initialRow = 0;
        int _initialColumn = 0;
        int _columnLimit = 4;
        int _maxItems = 15;
        bool _keepOnTop;

        List<EntityButton> _currentButtons = new List<EntityButton>();

        public MainWindow()
        {
            InitializeComponent();
            StateChanged += MainWindowStateChangeRaised;

            FillDisplay();
        }

        void FillDisplay()
        {
            foreach (var item in _currentButtons)
            {
                mainGrid.Children.Remove(item);
            }
            _currentButtons.Clear();

            var entities = DatabaseManager.Instance.GetEntitiesInFolder(_currentFolderId);
            int row = _initialRow;
            int column = _initialColumn;
            for (int i = 0; i < _maxItems; i++)
            {
                var entity = entities.FirstOrDefault(x => x.Position == i);
                var button = new EntityButton
                {
                    Padding = new Thickness(10)
                };
                button.SetEntity(entity);

                _currentButtons.Add(button);
                mainGrid.Children.Add(button);

                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);

                ++column;
                if (column > _columnLimit)
                {
                    ++row;
                    column = 0;
                }
            }
        }

        #region Window Functionality
        // Can execute
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        // State change
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MainWindowBorder.BorderThickness = new Thickness(8);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindowBorder.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }
        #endregion

        private void ImageToggle_Checked(object sender, RoutedEventArgs e)
        {
            _keepOnTop = true;
        }

        private void ImageToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            _keepOnTop = false;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (_keepOnTop)
            {
                Window window = (Window)sender;
                window.Topmost = true;
            }
        }
    }
}
