using DataBase;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RaktarKezelőMaui.View
{
    public partial class CategoryPage : ContentPage
    {
        private ObservableCollection<Category> categories = new();
        private Category selectedCategory;
        private ApplicationDbContext db = new();

        public CategoryPage()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            var categoryList = db.Categories.ToList();
            categories = new ObservableCollection<Category>(categoryList);
            CategoryList.ItemsSource = categories;
        }

        private void OnCategorySelected(object sender, SelectionChangedEventArgs e)
        {
            selectedCategory = e.CurrentSelection.FirstOrDefault() as Category;
            if (selectedCategory != null)
            {
                CategoryNameEntry.Text = selectedCategory.Name;
            }
        }

        private void OnAddClicked(object sender, EventArgs e)
        {
            var newCategory = new Category
            {
                Name = CategoryNameEntry.Text
            };

            db.Categories.Add(newCategory);
            db.SaveChanges();

            categories.Add(newCategory);
            ClearForm();
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            if (selectedCategory == null)
                return;

            selectedCategory.Name = CategoryNameEntry.Text;
            db.SaveChanges();

            LoadCategories();
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (selectedCategory != null)
            {
                db.Categories.Remove(selectedCategory);
                db.SaveChanges();
                categories.Remove(selectedCategory);
                selectedCategory = null;
                ClearForm();
            }
        }

        private void ClearForm()
        {
            CategoryNameEntry.Text = "";
            CategoryList.SelectedItem = null;
        }
    }
}
