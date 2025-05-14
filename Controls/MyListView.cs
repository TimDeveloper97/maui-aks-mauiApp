using Microsoft.Maui.Graphics;
using System.Collections;
using VSmauiApp.ViewModels;

namespace VSmauiApp.Controls;

public class ItemSelectedEventArgs : EventArgs
{
    public ItemSelectedEventArgs(object? v) { Value = v; }
    public object? Value { get; set; }
}

public class MyListView : ScrollView
{
    VerticalStackLayout _content = new VerticalStackLayout();

    protected virtual View CreateItemView() => new Grid();

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        propertyName: nameof(ItemsSource),
        returnType: typeof(IEnumerable),
        declaringType: typeof(MyListView),
        propertyChanged: BindableItemsSourceChanged);
    static void BindableItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var parent = (MyListView)bindable;
        parent.Children.Clear();

        if (parent.ItemsSource != null)
        {
            foreach (var e in parent.ItemsSource)
            {
                var item = parent.CreateItemView();
                item.BindingContext = e;

                item.RegisterClickEvent(() => {
                    parent.OnItemViewTapped(item);
                });
                parent.Children.Add(item);
            }
        }
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }
    static void BindableSelectedItemChanged(BindableObject bindableObject, object oldValue, object newValue)
    {
        ((MyListView)bindableObject).SelectedItemChanged?.Invoke(bindableObject, new ItemSelectedEventArgs(newValue));
    }
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        propertyName: nameof(SelectedItem),
        returnType: typeof(object),
        declaringType: typeof(MyListView),
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: BindableSelectedItemChanged);

    public event EventHandler<ItemSelectedEventArgs>? SelectedItemChanged;
    protected virtual void OnItemViewTapped(View view)
    {
        SelectedItem = view.BindingContext;
    }
    new public IList<IView> Children => _content.Children;
    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public double Spacing
    {
        get => _content.Spacing;
        set => _content.Spacing = value;
    }
    public MyListView()
    {
        this.Content = _content;
        this.SetBinding(ItemsSourceProperty,
            nameof(MainViewModel.ListItems));
        this.SetBinding(SelectedItemProperty, nameof(MainViewModel.ActiveItem));
    }
}

public class MyListView<T> : MyListView
    where T : View, new()
{
    protected override View CreateItemView() => new T();
}