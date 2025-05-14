
using System.Threading.Tasks;
using VSmauiApp.Controls;
using VSmauiApp.ViewModels;

namespace VSmauiApp.Views;

public partial class FilterSelector : ContentView
{
    public static readonly BindableProperty ModeProperty = BindableProperty.Create(
        nameof(Mode),
        typeof(char),
        typeof(FilterSelector),
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (s, o, n) => {
            ((FilterSelector)s).SetSelected((char)n);
        });
    public char Mode 
    { 
        get => (char)GetValue(ModeProperty); 
        set => SetValue(ModeProperty, value); 
    }

    public void ForEach(Func<Label, bool> callback)
    {
        foreach (View item in ((Grid)activeIndicator.Parent).Children)
        {
            if (item != activeIndicator && callback((Label)item) == false)
                break;
        }
    }

    void FindItem(char n, Action<Label> callback)
    {
        ForEach(label => {
            if (label.Text[5] == n)
            {
                callback(label);
                return false;
            }
            return true;
        });
    }
    void SetSelected(char n) => FindItem(n, SetSelected);
    void SetSelected(Label label)
    {
        activeIndicator.SetValue(Grid.ColumnProperty,
            label.GetValue(Grid.ColumnProperty));
    }
    public FilterSelector()
	{
		InitializeComponent();

        ForEach(label => {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => Mode = label.Text[5];
            
            label.GestureRecognizers.Add(tapGestureRecognizer);
            return true;
        });
	}

    private void EndDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        var filter = (StationDetailFilter)BindingContext;
        if (filter.Start > filter.End)
        {
            App.DisplayAlert("Khoảng thời gian không phù hợp");
        }
    }

    private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        var filter = (StationDetailFilter)BindingContext;
    }
}