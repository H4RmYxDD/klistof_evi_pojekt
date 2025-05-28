using RaktarKezeloMaui.Services;
using System;

namespace RaktarKezeloMaui.Views
{
    public partial class DeliveryPage : ContentPage
    {
        private readonly DeliveryService _deliveryService;
        private Guid _lastPackageId;

        public DeliveryPage()
        {
            InitializeComponent();
            _deliveryService = new DeliveryService();
        }

        private async void OnSendPackageClicked(object sender, EventArgs e)
        {
            var package = new Package
            {
                CustomerName = CustomerNameEntry.Text,
                Address = AddressEntry.Text,
                ZipCode = ZipCodeEntry.Text
            };

            try
            {
                var response = await _deliveryService.SendPackageAsync(package);
                _lastPackageId = response.Id;
                ResultLabel.Text = $"Csomag sikeresen elküldve. ID: {_lastPackageId}";
            }
            catch (Exception ex)
            {
                ResultLabel.Text = $"Hiba: {ex.Message}";
            }
        }

        private async void OnGetStatusClicked(object sender, EventArgs e)
        {
            if (_lastPackageId == Guid.Empty)
            {
                ResultLabel.Text = "Nincs elküldött csomag azonosító.";
                return;
            }

            try
            {
                var statusResponse = await _deliveryService.GetStatusAsync(_lastPackageId);
                ResultLabel.Text = $"Csomag státusza: {statusResponse.Status}";
            }
            catch (Exception ex)
            {
                ResultLabel.Text = $"Hiba: {ex.Message}";
            }
        }
    }
}
