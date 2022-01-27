using KEKWSoundboard.Database;
using KEKWSoundboard.Windows;
using NAudio.CoreAudioApi;
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
    /// Interaction logic for PreferencesPage.xaml
    /// </summary>
    public partial class PreferencesPage : PopupPage
    {
        List<string> _renderDeviceIds = new List<string>();
        List<string> _captureDeviceIds = new List<string>();

        DatabasePreferences _preferences;

        public PreferencesPage()
        {
            InitializeComponent();

            _preferences = DatabaseManager.Instance.GetPreferences();

            cmbPrimaryRenderDevices.Items.Add("None");
            cmbSecondaryRenderDevices.Items.Add("None");
            cmbCaptureDevices.Items.Add("None");

            // Get all of the render devices and fill the ComboBox
            var enumerator = new MMDeviceEnumerator();
            foreach (var wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                _renderDeviceIds.Add(wasapi.ID);
                cmbPrimaryRenderDevices.Items.Add(wasapi.FriendlyName);
                cmbSecondaryRenderDevices.Items.Add(wasapi.FriendlyName);
            }

            foreach (var wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                _captureDeviceIds.Add(wasapi.ID);
                cmbCaptureDevices.Items.Add(wasapi.FriendlyName);
            }

            // Set the selected item to the current device
            cmbPrimaryRenderDevices.SelectedIndex = _renderDeviceIds.IndexOf(_preferences.PrimaryRenderDevice);
            cmbSecondaryRenderDevices.SelectedIndex = _renderDeviceIds.IndexOf(_preferences.SecondaryRenderDevice);
            cmbCaptureDevices.SelectedIndex = _captureDeviceIds.IndexOf(_preferences.CaptureDevice);

            // Set the volume sliders
            sldPrimaryVolume.Value = _preferences.PrimaryRenderDeviceVolume;
            sldSecondaryVolume.Value = _preferences.SecondaryRenderDeviceVolume;
            sldInputVolume.Value = _preferences.CaptureDeviceVolume;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Update the device ID's
            _preferences.PrimaryRenderDevice = _renderDeviceIds[cmbPrimaryRenderDevices.SelectedIndex];
            _preferences.SecondaryRenderDevice = _renderDeviceIds[cmbSecondaryRenderDevices.SelectedIndex];
            _preferences.CaptureDevice = _captureDeviceIds[cmbCaptureDevices.SelectedIndex];

            // Update the volumes
            _preferences.PrimaryRenderDeviceVolume = (float)sldPrimaryVolume.Value;
            _preferences.SecondaryRenderDeviceVolume = (float)sldSecondaryVolume.Value;
            _preferences.CaptureDeviceVolume = (float)sldInputVolume.Value;

            DatabaseManager.Instance.UpdatePreferences(_preferences);

            PopupWindow.SetResult(true);
            MainWindow.Instance.ReloadCaptureDevice();
        }
    }
}
