using KEKWSoundboard.Database;
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
    /// Interaction logic for NewItemPage.xaml
    /// </summary>
    public partial class NewItemPage : PopupPage
    {
        public const int MaxFolderDepth = 1;

        int _position;

        public NewItemPage(int position)
        {
            _position = position;

            InitializeComponent();

            var depth = GetFolderDepth();
            if (depth >= MaxFolderDepth)
            {
                folderButton.IsEnabled = false;
            }
        }

        private void PopupPage_Loaded(object sender, RoutedEventArgs e)
        {
            PopupWindow.Title = "New Element";
        }

        private void SoundButton_Click(object sender, RoutedEventArgs e)
        {
            var sound = new DatabaseSound
            {
                ParentId = MainPage.CurrentFolderId,
                Position = _position
            };

            var soundPage = new EditSoundPage();

            PopupWindow.SetPage(soundPage);

            soundPage.SetSound(sound);
        }

        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPage = new EditFolderPage()
            {
                Folder = new DatabaseFolder
                {
                    ParentId = MainPage.CurrentFolderId,
                    Position = _position
                }
            };

            PopupWindow.SetPage(folderPage);
        }

        int GetFolderDepth()
        {
            int depth = 0;
            int? parentId = MainPage.CurrentFolderId;
            while (parentId != null)
            {
                ++depth;
                parentId = DatabaseManager.Instance.GetEntity(parentId.Value).ParentId;
            }
            return depth;
        }
    }
}
