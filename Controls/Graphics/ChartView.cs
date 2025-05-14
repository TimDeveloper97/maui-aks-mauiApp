using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSmauiApp.Controls.Graphics
{

    public class ChartInfo
    {
        public string? Title { get; set; }
        public AxisInfo Ox { get; set; } = new AxisInfo();
        public AxisInfo Oy { get; set; } = new AxisInfo();

        public Color Stroke { get; set; } = Color.FromArgb("#624BFD");
        public double StrokeThickness { get; set; } = 1;
    }

    public class ChartTitle : Label
    {
        public ChartTitle() 
        {
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
            Margin = new Thickness(10);
        }
    }
    public class ChartView : ContentView
    {
        public object? Title 
        { 
            get => _title.Text; 
            set => _title.Text = value?.ToString(); 
        }

        Grid _body = new Grid();
        HorizontalAxis _xAxis = new HorizontalAxis();
        VerticalAxis _yAxis = new VerticalAxis();
        ChartArea _area = new ChartArea();
        ChartTitle _title = new ChartTitle();

        public AxisInfo Ox
        {
            get => _xAxis.Info;
            set => _xAxis.BindingContext = value;
        }
        public AxisInfo Oy
        {
            get => _yAxis.Info;
            set => _yAxis.BindingContext = value;
        }
        public void Axis(string name, Action<Axis> action)
        {
            action?.Invoke(name[1] == 'x' ? _xAxis : _yAxis);
        }
        public void Axis(Action<Axis, Axis> action)
        {
            action(_xAxis, _yAxis);
        }
        public ChartArea Area => _area;
        public IList<IView> Plots => _area.Plots;
        public PlotView this[int index] => (PlotView)Plots[index];
        protected override Size MeasureOverride(double w, double h)
        {
            var sz = base.MeasureOverride(w, h);
            
            Oy.Measure(HeightRequest - _title.DesiredSize.Height - _xAxis.DesiredSize.Height);
            Ox.Measure(w - (Margin.Left + Margin.Right + Padding.Left + Padding.Right) - _yAxis.DesiredSize.Width);

            foreach (PlotView plot in Plots)
            {
                plot.Refresh();
            }
            return sz;
        }
        protected override void OnBindingContextChanged()
        {
            this.Binding<ChartInfo>(i => {
                base.OnBindingContextChanged();
            });
        }
        public ChartView()
        {
            //this.RegisterAutoBinding();

            HeightRequest = 300;
            Background = Colors.White;

            Content = _body;

            _body.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            _body.RowDefinitions.Add(new RowDefinition());
            _body.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            _body.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            _body.ColumnDefinitions.Add(new ColumnDefinition());

            _area.Background = Colors.White;

            _body.Add(_title);
            _body.SetColumnSpan(_title, 2);

            _body.Add(_xAxis, 1, 2);
            _body.Add(_yAxis, 0, 1);

            _body.Add(_area, 1, 1);

        }
    }
}
