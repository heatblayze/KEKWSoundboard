using KEKWSoundboard.Database;
using Microsoft.Win32;
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
    /// Interaction logic for EditFolderPage.xaml
    /// </summary>
    public partial class EditFolderPage : PopupPage
    {
        public DatabaseFolder Folder { get; set; }

        public EditFolderPage()
        {
            InitializeComponent();
        }

        private void PopupPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Folder.Id >= 0)
            {
                PopupWindow.Title = "Edit Folder";
            }
            else
            {
                PopupWindow.Title = "New Folder";
            }
        }

        private void txtIcon_TextChanged(object sender, TextChangedEventArgs e)
        {
            var path = System.IO.Path.GetFullPath(txtIcon.Text);
            var rawData = System.IO.File.ReadAllBytes(path);
            var image = (BitmapSource)new ImageSourceConverter().ConvertFrom(rawData);
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Folder.ImageFile = txtIcon.Text;
            Folder.Name = txtName.Text;

            bool success = false;
            if (Folder.Id >= 0)
            {
                // Editing folder
                success = DatabaseManager.Instance.UpdateFolder(Folder);
            }
            else
            {
                // New folder
                success = DatabaseManager.Instance.AddFolder(Folder);
            }

            if (success)
            {
                PopupWindow.SetResult(true);
            }
        }        
    }
}
