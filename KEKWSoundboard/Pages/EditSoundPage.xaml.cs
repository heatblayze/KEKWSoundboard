using KEKWSoundboard.Database;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace KEKWSoundboard.Pages
{
    /// <summary>
    /// Interaction logic for EditSoundPage.xaml
    /// </summary>
    public partial class EditSoundPage : Page
    {
        public DatabaseSound Sound { get; private set; }

        public EditSoundPage()
        {
            InitializeComponent();
        }

        public void SetSound(DatabaseSound sound)
        {
            Sound = sound;

            if (Sound.Id >= 0)
            {
                // Editing sound
                MainWindow.Instance.SetPageTitle("Edit Sound");

                txtIcon.Text = sound.ImageFile;
                txtSound.Text = sound.SoundFile;
                txtName.Text = sound.Name;
                sldVolume.Value = sound.Volume;
            }
            else
            {
                // New sound
                MainWindow.Instance.SetPageTitle("New Sound");
            }
        }

        private void txtIcon_TextChanged(object sender, TextChangedEventArgs e)
        {
            var image = new BitmapImage(new Uri(txtIcon.Text));
            imgIcon.Source = image;

            iconAspectFitter.AspectRatio = image.Width / image.Height;
        }

        private void btnIconSelect_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.FileName = txtIcon.Text;
            dlg.Filter = "Image Files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp";
            if (dlg.ShowDialog() == true)
            {
                txtIcon.Text = dlg.FileName;
            }
        }

        private void txtSound_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnSoundSelect_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.FileName = txtSound.Text;
            dlg.Filter = "Audio Files (*.wav;*.aiff;*.mp3)|*.wav;*.aiff;*.mp3";
            if (dlg.ShowDialog() == true)
            {
                txtSound.Text = dlg.FileName;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Sound.ImageFile = txtIcon.Text;
            Sound.SoundFile = txtSound.Text;
            Sound.Volume = (float)sldVolume.Value;
            Sound.Name = txtName.Text;

            bool success = false;
            if (Sound.Id >= 0)
            {
                // Editing sound
                success = DatabaseManager.Instance.UpdateSound(Sound);
            }
            else
            {
                // New sound
                success = DatabaseManager.Instance.AddSound(Sound);
            }

            if (success)
            {
                MainWindow.Instance.NavigateToPage(PageType.Main);
            }
        }
    }
}
