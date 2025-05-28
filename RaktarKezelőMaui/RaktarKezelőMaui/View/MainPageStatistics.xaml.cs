using DataBase;
using Microcharts;
using RaktarKezelõMaui.ViewModel;
using SkiaSharp;

namespace RaktarKezelőMaui.View;

public partial class MainPageStatistics : ContentPage
{
    public MainPageStatistics(ApplicationDbContext dbContext)
    {
        InitializeComponent();
        BindingContext = new MainPageStatisticsViewModel(dbContext);

        // Daily Orders
        var color = SKColor.Parse("#266489");
        var dailyData = dbContext.Purchases
            .GroupBy(p => p.BuyingTime.Date)
            .OrderBy(g => g.Key)
            .Select(g => new ChartEntry(g.Count())
            {
                Label = g.Key.ToString("MM.dd"),
                ValueLabel = g.Count().ToString(),
                Color = color, // Use the local variable here
                TextColor = SKColors.Black,
                ValueLabelColor = SKColors.DarkSlateGray
            }).ToArray();

        dailyChartView.Chart = new BarChart
        {
            Entries = dailyData,
            LabelTextSize = 24,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelOrientation = Orientation.Horizontal,
            BackgroundColor = SKColors.White,
            Margin = 20
        };

        // Monthly Orders
        var monthlyData = dbContext.Purchases
            .GroupBy(p => new { p.BuyingTime.Year, p.BuyingTime.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new ChartEntry(g.Count())
            {
                Label = $"{g.Key.Year}.{g.Key.Month:D2}",
                ValueLabel = g.Count().ToString(),
                Color = color,
                TextColor = SKColors.Black,
                ValueLabelColor = SKColors.DarkSlateGray
            }).ToArray();

        monthlyChartView.Chart = new BarChart
        {
            Entries = monthlyData,
            LabelTextSize = 24,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelOrientation = Orientation.Horizontal,
            BackgroundColor = SKColors.White,
            Margin = 20
        };

        // Yearly Orders
        var yearlyData = dbContext.Purchases
            .GroupBy(p => p.BuyingTime.Year)
            .OrderBy(g => g.Key)
            .Select(g => new ChartEntry(g.Count())
            {
                Label = g.Key.ToString(),
                ValueLabel = g.Count().ToString(),
                Color = color,
                TextColor = SKColors.Black,
                ValueLabelColor = SKColors.DarkSlateGray
            }).ToArray();

        yearlyChartView.Chart = new BarChart
        {
            Entries = yearlyData,
            LabelTextSize = 24,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelOrientation = Orientation.Horizontal,
            BackgroundColor = SKColors.White,
            Margin = 20
        };
    }
}