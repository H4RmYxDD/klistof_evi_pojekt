using DataBase;
using RaktarKezelőMaui.ViewModel;

namespace RaktarKezelőMaui.View;

public partial class MainPageStatistics : ContentPage
{
    public MainPageStatistics(ApplicationDbContext dbContext)
    {
        InitializeComponent();
        BindingContext = new MainPageStatisticsViewModel(dbContext);
    }
}
