using KEKWSoundboard.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KEKWSoundboard.Windows
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        public PopupPage PopupPage { get; set; }

        public PopupWindow()
        {
            InitializeComponent();
        }

        public void SetPage<T>(T page) where T : PopupPage
        {
            page.PopupWindow = this;
            contentFrame.Navigate(page);
        }

        #region Chrome Window Functionality
        // Can execute
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
        #endregion

        public void SetResult(bool result)
        {
            DialogResult = result;
        }
    }
}
