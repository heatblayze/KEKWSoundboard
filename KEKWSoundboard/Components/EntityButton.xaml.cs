using KEKWSoundboard.Audio;
using KEKWSoundboard.Database;
using KEKWSoundboard.Pages;
using KEKWSoundboard.Windows;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

                AspectRatioLayoutDecorator aspectFitter = null;
                Image image = null;

                switch (entity)
                {
                    case DatabaseSound:
                        aspectFitter = iconAspectFitter;
                        image = imgIcon;
                        folderGrid.Visibility = Visibility.Hidden;
                        break;
                    case DatabaseFolder:
                        aspectFitter = folderIconAspectFitter;
                        image = folderIcon;
                        break;
                }

                if (!string.IsNullOrEmpty(entity.ImageFile))
                {
                    var path = System.IO.Path.GetFullPath(entity.ImageFile);
                    var rawData = System.IO.File.ReadAllBytes(path);
                    var imageSource = (BitmapSource)new ImageSourceConverter().ConvertFrom(rawData);
                    image.Source = imageSource;

                    aspectFitter.AspectRatio = imageSource.Width / imageSource.Height;
                }
            }
            else
            {
                btnPlay.Visibility = Visibility.Hidden;
                folderGrid.Visibility = Visibility.Hidden;
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            var page = new NewItemPage(Position);

            var popup = new PopupWindow();
            popup.Owner = MainWindow.Instance;
            popup.SetPage(page);

            var result = popup.ShowDialog();
            if (result == true)
            {
                MainWindow.Instance.RefreshContent();
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Play!");

            switch(Entity)
            {
                case DatabaseSound sound:
                    AudioPlaybackManager.PlaySound(sound.SoundFile, sound.Volume);
                    break;
                case DatabaseFolder:
                    MainPage.CurrentFolderId = Entity.Id;
                    break;
            }            
        }

        private void btnPlay_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var soundPage = new EditSoundPage();

            var popup = new PopupWindow();
            popup.Owner = MainWindow.Instance;
            popup.SetPage(soundPage);

            soundPage.SetSound(Entity as DatabaseSound);

            var result = popup.ShowDialog();
            if (result == true)
            {
                MainWindow.Instance.RefreshContent();
            }
        }
    }
}
