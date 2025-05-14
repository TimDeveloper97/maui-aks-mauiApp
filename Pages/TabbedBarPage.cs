namespace VSmauiApp.Pages;

using System.Collections;
using VSmauiApp.Controls;
using TABBAR = VSmauiApp.Controls.TabBar;
public abstract class TabbedBarPage : ContentPage
{
    ContentView body = new ContentView();

    protected virtual View CreateBodyCore(string name)
    {
        return new Grid();
    }
    protected void SetBody(TabItem item)
    {
        var vm = (TabItemInfo)item.BindingContext;

        View? view = null;
        var type = vm.ContentViewType;
        if (type != null)
        {
            view = BaseViewModel.GetObject<View>(type); 
        }
        if (view == null)
        {
            view = CreateBodyCore(vm.Route);
        }
        body.Content = view;

        type = vm.ContextType;
        if (type != null)
        {
            var context = BaseViewModel.GetObject<object>(type);
            this.BindingContext = context;
        }
    }
    public TabbedBarPage()
    {
        var tabbar = new TABBAR();
        var grid = new Grid {
            RowDefinitions = {
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            },
            Children = { body, tabbar },
        };

        tabbar.SetValue(Grid.RowProperty, 1);
        tabbar.ItemsSource = CreateTabBarItems();

        tabbar.SelectedRouteChanged += (s, e) => {
            SetBody((TabItem)s);
        };

        var first = tabbar.GetItemAt(0);
        first.IsActivated = true;
        SetBody(first);

        Content = grid;

        this.SetBinding(TitleProperty, nameof(Title));
    }

    protected abstract IEnumerable CreateTabBarItems();
}