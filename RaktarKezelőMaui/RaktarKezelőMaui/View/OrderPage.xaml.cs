using DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RaktarKezelőMaui.View
{
    public partial class OrderPage : ContentPage
    {
        private ObservableCollection<Purchase> purchases = new();
        private Purchase selectedPurchase;
        private ApplicationDbContext db = new();

        public OrderPage()
        {
            InitializeComponent();
            LoadPurchases();
        }

        private void LoadPurchases()
        {
            var purchaseList = db.Purchases
                .Include(p => p.PurchaseProducts)
                .ThenInclude(pp => pp.Product)
                .ToList();

            purchases = new ObservableCollection<Purchase>(purchaseList);
            PurchaseList.ItemsSource = purchases;
        }

        private void OnPurchaseSelected(object sender, SelectionChangedEventArgs e)
        {
            selectedPurchase = e.CurrentSelection.FirstOrDefault() as Purchase;
            if (selectedPurchase != null)
            {
                BuyerNameEntry.Text = selectedPurchase.BuyerName;
                BuyingDatePicker.Date = selectedPurchase.BuyingTime;
                StatusPicker.SelectedItem = selectedPurchase.PurchaseStatus.ToString();
                ProductInPurchaseList.ItemsSource = selectedPurchase.PurchaseProducts;
            }
        }

        private void OnChangeStatusClicked(object sender, EventArgs e)
        {
            if (selectedPurchase == null || StatusPicker.SelectedItem == null)
                return;

            var newStatusString = StatusPicker.SelectedItem.ToString();
            if (!Enum.TryParse(newStatusString, out Purchase.Status newStatus))
                return;

            var oldStatus = selectedPurchase.PurchaseStatus;
            if (oldStatus == newStatus)
                return;

            foreach (var pp in selectedPurchase.PurchaseProducts)
            {
                var product = db.Products.FirstOrDefault(p => p.Id == pp.ProductId);
                if (product == null) continue;

                if (oldStatus != Purchase.Status.Delivered && newStatus == Purchase.Status.Delivered)
                {
                    product.Quantity -= pp.Quantity;
                }
                else if (oldStatus == Purchase.Status.Delivered && newStatus != Purchase.Status.Delivered)
                {
                    product.Quantity += pp.Quantity;
                }
            }

            selectedPurchase.PurchaseStatus = newStatus;
            db.SaveChanges();

            LoadPurchases();
        }
    }
}
