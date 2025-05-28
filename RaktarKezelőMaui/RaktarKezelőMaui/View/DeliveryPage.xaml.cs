using RaktarKezeloMaui.Services;
using System;
using System.Collections.ObjectModel;

namespace RaktarKezelőMaui.View
{
    public partial class DeliveryPage : ContentPage
    {

        private readonly DeliveryService _apiService = new();
        private Guid _lastPackageId;

        // Add an observable collection for data binding
        public ObservableCollection<Package> Packages { get; } = new();

        public DeliveryPage()
        {
            InitializeComponent();
            LoadPackages();
        }

        private async void LoadPackages()
        {
            try
            {
                var packages = await _apiService.GetPackagesAsync();
                Packages.Clear();
                foreach (var pkg in packages)
                    Packages.Add(pkg);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnDeletePackageClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is Package pkg)
            {
                try
                {
                    await _apiService.DeletePackageAsync(pkg.Id);
                    Packages.Remove(pkg);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }

        private async void OnSendPackageClicked(object sender, EventArgs e)
        {
            var package = new Package
            {
                CustomerName = CustomerNameEntry.Text ?? string.Empty,
                Address = AddressEntry.Text ?? string.Empty,
                ZipCode = ZipCodeEntry.Text ?? string.Empty
            };

            try
            {
                var result = await _apiService.SendPackageAsync(package);
                if (result != null)
                {
                    _lastPackageId = result.Id;
                    ResultLabel.Text = $"Csomag elküldve!\nID: {result.Id}\nNév: {result.CustomerName}\nCím: {result.Address}\nIrányítószám: {result.ZipCode}";
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hiba", ex.Message, "OK");
                ResultLabel.Text = "Hiba történt a csomag küldésekor.";
            }
        }

        private async void OnGetStatusClicked(object sender, EventArgs e)
        {
            if (_lastPackageId == Guid.Empty)
            {
                ResultLabel.Text = "Nincs elküldött csomag azonosító.";
                return;
            }

            var status = await _apiService.GetStatusAsync(_lastPackageId);
            if (status != null)
            {
                ResultLabel.Text = $"Csomag státusza: {status}";
            }
            else
            {
                ResultLabel.Text = "Hiba történt a státusz lekérdezésekor.";
            }
        }
    }
}
