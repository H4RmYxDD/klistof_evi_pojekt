using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DataBase;
using Microsoft.EntityFrameworkCore;

namespace RaktarKezelõMaui.ViewModel
{
    public class PurchaseStat
    {
        public string Period { get; set; }
        public int Count { get; set; }
        public double TotalValue { get; set; }
    }

    public class MainPageStatisticsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<PurchaseStat> DailyStats { get; } = new();
        public ObservableCollection<PurchaseStat> MonthlyStats { get; } = new();
        public ObservableCollection<PurchaseStat> YearlyStats { get; } = new();

        public MainPageStatisticsViewModel(ApplicationDbContext dbContext)
        {
            LoadStats(dbContext);
        }

        private void LoadStats(ApplicationDbContext dbContext)
        {
            var purchases = dbContext.Purchases
                .Include(p => p.Products)
                .ToList();

            // Napi bontás
            var daily = purchases
                .GroupBy(p => p.BuyingTime.Date)
                .OrderByDescending(g => g.Key)
                .Select(g => new PurchaseStat
                {
                    Period = g.Key.ToString("yyyy-MM-dd"),
                    Count = g.Count(),
                    TotalValue = g.Sum(p => p.Products.Sum(prod => prod.PriceHuf * prod.Quantity))
                });
            DailyStats.Clear();
            foreach (var stat in daily)
                DailyStats.Add(stat);

            // Havi bontás
            var monthly = purchases
                .GroupBy(p => new { p.BuyingTime.Year, p.BuyingTime.Month })
                .OrderByDescending(g => g.Key.Year).ThenByDescending(g => g.Key.Month)
                .Select(g => new PurchaseStat
                {
                    Period = $"{g.Key.Year}-{g.Key.Month:00}",
                    Count = g.Count(),
                    TotalValue = g.Sum(p => p.Products.Sum(prod => prod.PriceHuf * prod.Quantity))
                });
            MonthlyStats.Clear();
            foreach (var stat in monthly)
                MonthlyStats.Add(stat);

            // Éves bontás
            var yearly = purchases
                .GroupBy(p => p.BuyingTime.Year)
                .OrderByDescending(g => g.Key)
                .Select(g => new PurchaseStat
                {
                    Period = g.Key.ToString(),
                    Count = g.Count(),
                    TotalValue = g.Sum(p => p.Products.Sum(prod => prod.PriceHuf * prod.Quantity))
                });
            YearlyStats.Clear();
            foreach (var stat in yearly)
                YearlyStats.Add(stat);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
