using KEKWSoundboard.Components;
using KEKWSoundboard.Database;
using KEKWSoundboard.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KEKWSoundboard.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public static int? CurrentFolderId
        {
            get
            {
                return _folderId;
            }
            set
            {
                var oldVal = _folderId;
                _folderId = value;
                MainWindow.Instance.SetBackButtonVisible(_folderId != null);

                if (oldVal != _folderId)
                {
                    MainWindow.Instance.RefreshContent();
                }
            }
        }
        private static int? _folderId;

        int _initialRow = 0;
        int _initialColumn = 0;
        int _columnLimit = 4;
        int _maxItems = 15;

        List<EntityButton> _currentButtons = new List<EntityButton>();

        public MainPage()
        {
            InitializeComponent();
        }

        public void FillDisplay()
        {
            foreach (var item in _currentButtons)
            {
                mainGrid.Children.Remove(item);
            }
            _currentButtons.Clear();

            var entities = DatabaseManager.Instance.GetEntitiesInFolder(CurrentFolderId);
            int row = _initialRow;
            int column = _initialColumn;
            for (int i = 0; i < _maxItems; i++)
            {
                var entity = entities.FirstOrDefault(x => x.Position == i);
                var button = new EntityButton
                {
                    Padding = new Thickness(10),
                    Position = i,
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

        public void Back()
        {
            if (CurrentFolderId.HasValue)
                CurrentFolderId = DatabaseManager.Instance.GetEntity(CurrentFolderId.Value).ParentId;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FillDisplay();
        }
    }
}
