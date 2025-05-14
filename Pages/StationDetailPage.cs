using System.Collections;

namespace VSmauiApp.Pages;
using Views;
using ViewModels;
public class StationDetailPage : TabbedBarPage
{
	public StationDetailPage()
	{
	}

    protected override IEnumerable CreateTabBarItems()
    {
        return new List<TabItemInfo> { 
            new TabItemInfo("Tức thời", "now", "bolt", typeof(StationNowView), typeof(StationRealTimeViewModel)),
            new TabItemInfo("Tra cứu", "status", "bar-chart", typeof(StationStatusView), typeof(StationStatusViewModel)),
            new TabItemInfo("Lịch sử", "history", "history", typeof(StationHistoryView), typeof(StationHistoryViewModel)),
            new TabItemInfo("Cài đặt", "setting", "setting", typeof(StationSettingView), typeof(StationSettingViewModel)),
        };
    }
}