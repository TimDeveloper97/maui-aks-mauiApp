using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using System.Collections;
using Vst.Controls.SVG;


namespace VSmauiApp
{
    public record TabItemInfo(string Text, string Route, string Icon, Type? ContentViewType = null, Type? ContextType = null);
}

namespace VSmauiApp.Controls
{
    public partial class TabItem : ContentView
    {
        public TabItem()
        {
            InitializeComponent();

            this.RegisterClickEvent(() =>
            {
                if (IsActivated) { return; }
                Dispatcher.StartTimer(TimeSpan.FromMilliseconds(50), () =>
                {
                    shadow.Opacity += 0.07;
                    if (shadow.Opacity >= 0.2)
                    {
                        Click?.Invoke(this, EventArgs.Empty);

                        shadow.Opacity = 0;
                        return false;
                    }
                    return true;
                });
            });
        }

        public static readonly BindableProperty RouteProperty = BindableProperty.Create(
            propertyName: nameof(Route),
            returnType: typeof(string),
            declaringType: typeof(TabItem),
            defaultValue: string.Empty
            );
        public string Route
        {
            get => (string)GetValue(RouteProperty);
            set => SetValue(RouteProperty, value);
        }
        public string Text
        {
            get => title.Text;
            set => title.Text = value;
        }
        public string Icon
        {
            get => icon.Source;
            set => icon.Source = value;
        }

        bool _ac;
        public bool IsActivated
        {
            get => _ac;
            set
            {
                if (_ac != value)
                {
                    _ac = value;
                    SetColor();
                }
            }
        }
        protected override void OnParentChanged()
        {
            base.OnParentChanged();
            if (Parent != null)
                SetColor();
        }
        void SetColor()
        {
            var tb = (TabBar)this.Parent;
            var color = _ac ? tb.ActivatedColor : tb.DefaultColor;

            title.TextColor = color;
            icon.Fill = color;
        }

        public event EventHandler? Click;
    }
    public partial class TabBar : Grid
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            propertyName: nameof(ItemsSource),
            returnType: typeof(IEnumerable),
            declaringType: typeof(TabBar),
            propertyChanged: ItemsSourceChanged);
        public static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tab = (TabBar)bindable;
            tab.Children.Clear();

            foreach (var e in tab.ItemsSource)
            {
                var item = new TabItem
                {
                    BindingContext = e,
                };
                tab.Children.Add(item);
            }
            tab.SetSelectedRoute();
        }
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        public TabItem GetItemAt(int index)
        {
            return (TabItem)Children[index];
        }
        public TabBar()
        {
            ChildAdded += (s, e) =>
            {
                var item = (TabItem)e.Element;
                ColumnDefinitions.Add(new ColumnDefinition());
                item.SetValue(Grid.ColumnProperty, Children.Count - 1);

                item.Click += (_, __) =>
                {
                    SelectedRoute = item.Route;
                    SelectedRouteChanged?.Invoke(_, EventArgs.Empty);
                };
            };
        }

        public static readonly BindableProperty SelectedRouteProperty = BindableProperty.Create(
            nameof(SelectedRoute),
            typeof(string),
            typeof(TabBar),
            propertyChanged: (s, o, n) => ((TabBar)s).SetSelectedRoute((string)n));

        public event EventHandler? SelectedRouteChanged;
        public string SelectedRoute
        {
            get => (string)GetValue(SelectedRouteProperty);
            set => SetValue(SelectedRouteProperty, value);
        }
        void SetSelectedRoute(string? route = null)
        {
            if (route != null)
            {
                foreach (TabItem item in Children)
                {
                    item.IsActivated = (item.Route == route);
                }
            }
        }

        public static readonly BindableProperty ActivatedColorProperty = BindableProperty.Create(
            propertyName: nameof(ActivatedColor),
            returnType: typeof(Color),
            declaringType: typeof(TabBar),
            defaultValue: Colors.Orange);
        public Color ActivatedColor
        {
            get => (Color)GetValue(ActivatedColorProperty);
            set => SetValue(ActivatedColorProperty, value);
        }

        public static readonly BindableProperty DefaultColorProperty = BindableProperty.Create(
            propertyName: nameof(DefaultColor),
            returnType: typeof(Color),
            declaringType: typeof(TabBar),
            defaultValue: Colors.Gray);
        public Color DefaultColor
        {
            get => (Color)GetValue(DefaultColorProperty);
            set => SetValue(DefaultColorProperty, value);
        }
    }
}

