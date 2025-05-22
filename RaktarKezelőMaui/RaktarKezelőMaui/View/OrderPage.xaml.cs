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
        private ObservableCollection<PurchaseProduct> newPurchaseProducts = new();
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

                this.BindingContext = selectedPurchase;
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

        private void OnAddProductToNewOrder(object sender, EventArgs e)
        {
            if (ProductPicker.SelectedIndex == -1 || string.IsNullOrWhiteSpace(QuantityEntry.Text))
            {
                DisplayAlert("Hiba", "Válassz terméket és add meg a mennyiséget!", "OK");
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

            var existing = newPurchaseProducts.FirstOrDefault(p => p.ProductId == selectedProduct.Id);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                newPurchaseProducts.Add(new PurchaseProduct
                {
                    ProductId = selectedProduct.Id,
                    Product = selectedProduct,
                    Quantity = quantity
                });
            }

            ProductInPurchaseList.ItemsSource = null;
            ProductInPurchaseList.ItemsSource = newPurchaseProducts;

            QuantityEntry.Text = "";
            ProductPicker.SelectedIndex = -1;
        }

        private void OnAddPurchaseClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BuyerNameEntry.Text) || newPurchaseProducts.Count == 0)
            {
                DisplayAlert("Hiba", "Tölts ki minden mezőt és adj hozzá legalább egy terméket!", "OK");
                return;
            }

            double totalPrice = newPurchaseProducts.Sum(p => p.Product.PriceHuf * p.Quantity);

            var newPurchase = new Purchase
            {
                BuyerName = BuyerNameEntry.Text,
                BuyingTime = BuyingDatePicker.Date,
                PurchaseStatus = Purchase.Status.New,
                PurchaseProducts = new List<PurchaseProduct>(newPurchaseProducts),
                TotalPrice = totalPrice
            };

            db.Purchases.Add(newPurchase);
            db.SaveChanges();

            purchases.Add(newPurchase);
            DisplayAlert("Rendelés mentve", $"Összesen: {totalPrice:N0} Ft", "OK");

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
            newPurchaseProducts.Clear();
            this.BindingContext = null;
        }
    }
}
