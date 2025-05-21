using DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace RaktarKezelőMaui.View
{
    public partial class OrderPage : ContentPage
    {
        private ObservableCollection<Purchase> purchases = new();
        private Purchase selectedPurchase;
        private ApplicationDbContext db = new();
        private List<Product> allProducts = new();

        public OrderPage()
        {
            InitializeComponent();
            LoadPurchases();
            LoadProducts();
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

        private void LoadProducts()
        {
            allProducts = db.Products.ToList();
            ProductPicker.ItemsSource = allProducts.Select(p => p.Name).ToList();
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

        private void OnAddPurchaseClicked(object sender, EventArgs e)
        {
            var buyerName = BuyerNameEntry.Text;
            var date = BuyingDatePicker.Date;
            var status = Purchase.Status.New;

            if (string.IsNullOrWhiteSpace(buyerName) ||
                ProductPicker.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(QuantityEntry.Text))
            {
                DisplayAlert("Hiba", "Tölts ki minden mezőt!", "OK");
                return;
            }

            if (!double.TryParse(QuantityEntry.Text, out double quantity) || quantity <= 0)
            {
                DisplayAlert("Hiba", "Érvénytelen mennyiség.", "OK");
                return;
            }

            var selectedProductName = ProductPicker.SelectedItem.ToString();
            var selectedProduct = allProducts.FirstOrDefault(p => p.Name == selectedProductName);
            if (selectedProduct == null)
            {
                DisplayAlert("Hiba", "Termék nem található.", "OK");
                return;
            }

            var newPurchase = new Purchase
            {
                BuyerName = buyerName,
                BuyingTime = date,
                PurchaseStatus = status,
                PurchaseProducts = new List<PurchaseProduct>()
            };

            var purchaseProduct = new PurchaseProduct
            {
                ProductId = selectedProduct.Id,
                Quantity = quantity,
                Product = selectedProduct,
                Purchase = newPurchase
            };

            newPurchase.PurchaseProducts.Add(purchaseProduct);

            db.Purchases.Add(newPurchase);
            db.SaveChanges();

            purchases.Add(newPurchase);
            ClearForm();
        }

        private void ClearForm()
        {
            BuyerNameEntry.Text = "";
            QuantityEntry.Text = "";
            ProductPicker.SelectedIndex = -1;
            StatusPicker.SelectedIndex = -1;
            BuyingDatePicker.Date = DateTime.Today;
            ProductInPurchaseList.ItemsSource = null;
            selectedPurchase = null;
        }
    }
}
