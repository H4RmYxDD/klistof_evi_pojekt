using DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace RaktarKezelőMaui.View
{
    public partial class CategoryPage : ContentPage
    {
        private ObservableCollection<Category> categories = new();
        private ObservableCollection<Product> selectedProducts = new();
        private List<Product> allProducts = new();
        private Category selectedCategory;
        private ApplicationDbContext db = new();

        // Cache a módosított terméklistákhoz
        private Dictionary<int, List<Product>> categoryProductCache = new();

        public CategoryPage()
        {
            InitializeComponent();
            LoadCategories();
            LoadProducts();
        }

        private void LoadCategories()
        {
            var categoryList = db.Categories
                .Include(c => c.Products)
                .ToList();

            categories = new ObservableCollection<Category>(categoryList);
            CategoryList.ItemsSource = categories;
        }

        private void LoadProducts()
        {
            allProducts = db.Products.ToList();
            ProductPicker.ItemsSource = allProducts.Select(p => p.Name).ToList();
        }

        private void OnCategorySelected(object sender, SelectionChangedEventArgs e)
        {
            selectedCategory = e.CurrentSelection.FirstOrDefault() as Category;
            if (selectedCategory != null)
            {
                CategoryNameEntry.Text = selectedCategory.Name;

                if (categoryProductCache.ContainsKey(selectedCategory.Id))
                {
                    selectedProducts = new ObservableCollection<Product>(categoryProductCache[selectedCategory.Id]);
                }
                else
                {
                    selectedProducts = new ObservableCollection<Product>(
                        db.Categories
                          .Include(c => c.Products)
                          .FirstOrDefault(c => c.Id == selectedCategory.Id)?
                          .Products ?? new List<Product>()
                    );
                }

                ProductsInCategoryList.ItemsSource = selectedProducts;
            }
        }

        private void OnAddProductToCategory(object sender, EventArgs e)
        {
            if (ProductPicker.SelectedIndex == -1 || selectedCategory == null)
            {
                DisplayAlert("Hiba", "Válassz ki egy terméket és egy kategóriát!", "OK");
                return;
            }

            var selectedProductName = ProductPicker.SelectedItem.ToString();
            var productToAdd = allProducts.FirstOrDefault(p => p.Name == selectedProductName);

            if (productToAdd != null && !selectedProducts.Any(p => p.Id == productToAdd.Id))
            {
                selectedProducts.Add(productToAdd);

                if (categoryProductCache.ContainsKey(selectedCategory.Id))
                    categoryProductCache[selectedCategory.Id] = selectedProducts.ToList();
                else
                    categoryProductCache.Add(selectedCategory.Id, selectedProducts.ToList());
            }
        }

        private void OnRemoveProductFromCategory(object sender, EventArgs e)
        {
            var button = sender as Button;
            var productToRemove = button?.BindingContext as Product;

            if (productToRemove != null && selectedCategory != null)
            {
                selectedProducts.Remove(productToRemove);

                if (categoryProductCache.ContainsKey(selectedCategory.Id))
                    categoryProductCache[selectedCategory.Id] = selectedProducts.ToList();
            }
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            if (selectedCategory == null)
            {
                DisplayAlert("Hiba", "Nincs kiválasztva kategória!", "OK");
                return;
            }

            selectedCategory.Name = CategoryNameEntry.Text;

            var categoryInDb = db.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.Id == selectedCategory.Id);

            if (categoryInDb != null)
            {
                categoryInDb.Products.Clear();
                foreach (var p in selectedProducts)
                {
                    var productInDb = db.Products.Find(p.Id);
                    if (productInDb != null)
                    {
                        categoryInDb.Products.Add(productInDb);
                    }
                }

                db.SaveChanges();

                // Cache törlése mentés után
                if (categoryProductCache.ContainsKey(selectedCategory.Id))
                    categoryProductCache.Remove(selectedCategory.Id);

                LoadCategories();
            }
        }

        private void ClearForm()
        {
            if (selectedCategory != null && categoryProductCache.ContainsKey(selectedCategory.Id))
            {
                categoryProductCache.Remove(selectedCategory.Id);
            }

            CategoryNameEntry.Text = "";
            ProductPicker.SelectedIndex = -1;
            selectedProducts.Clear();
            ProductsInCategoryList.ItemsSource = null;
            CategoryList.SelectedItem = null;
        }
    }
}
