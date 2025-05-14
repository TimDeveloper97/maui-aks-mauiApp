
using System.Collections;
using VSmauiApp.ViewModels;
namespace VSmauiApp.Pages;
using Views;
using Controls;

public class MainPage : TabbedBarPage
{
    public MainPage()
	{
	}

    protected override IEnumerable CreateTabBarItems()
    {
        return new List<object> {
                new TabItemInfo("Trang chủ", "home", "home", typeof(MyListView<StationView>), typeof(StationsViewModel)),
                new TabItemInfo("Lịch sử", "history", "history"),
                new TabItemInfo("Cài đặt", "setting", "setting"),
            };
    }
}