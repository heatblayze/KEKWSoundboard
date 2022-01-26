using KEKWSoundboard.Database;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KEKWSoundboard.Components
{
    /// <summary>
    /// Interaction logic for EntityButton.xaml
    /// </summary>
    public partial class EntityButton : UserControl
    {
        public DatabaseEntity Entity { get; private set; }
        public int Position { get; set; }

        public EntityButton()
        {
            InitializeComponent();
        }

        public void SetEntity(DatabaseEntity entity)
        {
            // TODO: check the type of entity and display appropriate content
            Entity = entity;

            if (Entity != null)
            {
                btnNew.Visibility = Visibility.Hidden;
                btnNew.IsEnabled = false;

                if (!string.IsNullOrEmpty(entity.ImageFile))
                {
                    var path = System.IO.Path.GetFullPath(entity.ImageFile);
                    var image = new BitmapImage(new Uri(path));
                    imgIcon.Source = image;

                    iconAspectFitter.AspectRatio = image.Width / image.Height;
                    iconAspectFitter.AspectRatioMode = AspectRatioMode.EnvelopeParent;
                }
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.NavigateToPage(PageType.EditSound, new DatabaseSound
            {
                ParentId = MainPage.CurrentFolderId,
                Position = Position
            });
        }
    }
}
