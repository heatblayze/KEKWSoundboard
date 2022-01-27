using KEKWSoundboard.Audio;
using KEKWSoundboard.Database;
using KEKWSoundboard.Pages;
using System;
using System.Windows;
using System.Windows.Input;

namespace KEKWSoundboard.Windows
{
    public enum PageType
    {
        Main,
        EditEntity,
        Preferences
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        bool _keepOnTop;
        MainPage _mainPage;
        AudioCaptureManager _audioCaptureManager;

        public MainWindow()
        {
            Instance = this;

            InitializeComponent();
            StateChanged += MainWindowStateChangeRaised;

            _mainPage = new MainPage();
            contentFrame.Navigate(_mainPage);
            ReloadCaptureDevice();

            btnBack.Visibility = Visibility.Hidden;
        }

        public void SetBackButtonVisible(bool visible)
        {
            btnBack.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }

        public void RefreshContent()
        {
            contentFrame.Refresh();
        }

        public void ReloadCaptureDevice()
        {
            if (_audioCaptureManager != null)
            {
                _audioCaptureManager.Dispose();
            }

            if (DatabaseManager.CaptureDevice != null && DatabaseManager.PrimaryRenderDevice != null)
                _audioCaptureManager = new AudioCaptureManager(DatabaseManager.CaptureDevice, DatabaseManager.PrimaryRenderDevice, null, DatabaseManager.CaptureDeviceVolume);
        }

        #region Chrome Window Functionality
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
            KeepOnTop();
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        // State change
        private void MainWindowStateChangeRaised(object? sender, EventArgs e)
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

        #region Window Functionality

        private void Window_Activated(object sender, EventArgs e)
        {
            KeepOnTop();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            KeepOnTop();
        }

        void KeepOnTop()
        {
            if (_keepOnTop)
            {
                Topmost = false;
                Topmost = true;
            }
            else
            {
                Topmost = false;
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

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainPage.Back();
        }

        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            var preferencesPage = new PreferencesPage();
            var popup = new PopupWindow();
            popup.Owner = this;
            popup.Title = "Preferences";
            popup.SetPage(preferencesPage);

            var result = popup.ShowDialog();
            if (result == true)
            {
                ReloadCaptureDevice();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _audioCaptureManager?.Dispose();
        }
    }
}
