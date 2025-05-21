using DataBase;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RaktarKezelőMaui.View
{
    public partial class ProductPage : ContentPage
    {
        private ObservableCollection<Product> products = new();
        private Product selectedProduct;
        private ApplicationDbContext db = new();

        public ProductPage()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            var productList = db.Products.ToList();
            products = new ObservableCollection<Product>(productList);
            ProductList.ItemsSource = products;
        }

        private void OnProductSelected(object sender, SelectionChangedEventArgs e)
        {
            selectedProduct = e.CurrentSelection.FirstOrDefault() as Product;
            if (selectedProduct != null)
            {
                NameEntry.Text = selectedProduct.Name;
                PriceHufEntry.Text = selectedProduct.PriceHuf.ToString();
                PriceEurEntry.Text = selectedProduct.PirceEur.ToString();
                QuantityEntry.Text = selectedProduct.Quantity.ToString();
                MeasureUnitPicker.SelectedItem = selectedProduct.ProductMeasureUnit.ToString();
            }
        }

        private void OnAddClicked(object sender, EventArgs e)
        {
            var newProduct = new Product
            {
                Name = NameEntry.Text,
                PriceHuf = double.TryParse(PriceHufEntry.Text, out var huf) ? huf : 0,
                PirceEur = double.TryParse(PriceEurEntry.Text, out var eur) ? eur : 0,
                Quantity = double.TryParse(QuantityEntry.Text, out var qty) ? qty : 0,
                ProductMeasureUnit = Enum.TryParse(MeasureUnitPicker.SelectedItem?.ToString(), out Product.MeasureUnit mu) ? mu : Product.MeasureUnit.Piece
            };

            db.Products.Add(newProduct);
            db.SaveChanges();

            products.Add(newProduct);
            ClearForm();
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            if (selectedProduct == null)
                return;

            selectedProduct.Name = NameEntry.Text;
            selectedProduct.PriceHuf = double.TryParse(PriceHufEntry.Text, out var huf) ? huf : 0;
            selectedProduct.PirceEur = double.TryParse(PriceEurEntry.Text, out var eur) ? eur : 0;
            selectedProduct.Quantity = double.TryParse(QuantityEntry.Text, out var qty) ? qty : 0;
            selectedProduct.ProductMeasureUnit = Enum.TryParse(MeasureUnitPicker.SelectedItem?.ToString(), out Product.MeasureUnit mu) ? mu : Product.MeasureUnit.Piece;

            db.SaveChanges();
            LoadProducts();
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (selectedProduct != null)
            {
                db.Products.Remove(selectedProduct);
                db.SaveChanges();
                products.Remove(selectedProduct);
                selectedProduct = null;
                ClearForm();
            }
        }

        private void ClearForm()
        {
            NameEntry.Text = "";
            PriceHufEntry.Text = "";
            PriceEurEntry.Text = "";
            QuantityEntry.Text = "";
            MeasureUnitPicker.SelectedIndex = -1;
            ProductList.SelectedItem = null;
        }
    }
}
