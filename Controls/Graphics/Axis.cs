using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSmauiApp.Controls.Graphics
{
    public class TickInfo
    {
        public double Value { get; set; }
        public string? Text { get; set; }
    }
    public class AxisInfo
    {
        public double Min { get; set; } = double.MaxValue;
        public double Max { get; set; } = double.MinValue;
        public double Offset { get; set; } = 0;
        public double Scale { get; private set; }
        public double VisualSize { get; private set; } = 0;
        public virtual void Measure(double size)
        {
            VisualSize = size;
            if (Min >= Max)
            {
                Min = -1;
                Max = +1;
            }
            var d = Max - Min;
            if (Offset > 0) d += 2 * Offset;

            Scale = VisualSize / d;
        }

        public double Rotation { get; set; }
        public Func<double, string>? TickLabelCreator { get; set; } = (v) => v.ToString();
        public List<TickInfo> Ticks { get; set; } = new List<TickInfo>();

        double[]? _values;
        public double[] Values
        {
            get => _values ?? (_values = new double[0]);
            set
            {
                if (_values != value)
                {
                    _values = value;

                    Min = double.MaxValue;
                    Max = double.MinValue;

                    if (_values != null)
                    {
                        foreach (var v in _values)
                        {
                            if (Min > v) Min = v;
                            if (Max < v) Max = v;
                        }
                    }
                    OnValuesChanged();
                }
            }
        }

        protected virtual void OnValuesChanged()
        {
        }

        public double GetVisualValue(double value, bool invert = false)
        {
            if (VisualSize == 0) return 0;

            value -= Min;
            if (Offset > 0)
                value += Offset;

            value = Scale * (value - Min);
            if (invert) value = VisualSize - value;

            return value;
        }
        public AxisInfo CreateTicks(double min, double max, double step, double offset = 0)
        {
            if (Min > min) Min = min;
            if (Max < max) Max = max;
            Offset = offset;

            double x = min;
            while (max >= x)
            {
                Ticks.Add(new TickInfo { Value = x });
                x += step;
            }
            return this;
        }
    }
    public abstract class Axis : Grid, IRefreshOnBindingChanged
    {
        public AxisInfo Info => (AxisInfo)BindingContext;
        //double _scale = -1;
        //public double VisualSize { get; set; }
        //public double Max { get; set; }
        //public double Min { get; set; }
        //public Func<double, string>? TickLabelCreator { get; set; }
        //public IEnumerable? Ticks { get; set; }
        //public virtual double GetVisualValue(double value)
        //{
        //    if (VisualSize == 0) return 0;
        //    if (_scale < 0)
        //    {
        //        var d = Max - Min;
        //        if (0 >= d) return 0;

        //        _scale = VisualSize / d;
        //    }
        //    return _scale * (value - Min);
        //}
        
        public abstract double GetVisualValue(double value);
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            this.Refresh();
        }
        public void Refresh()
        {
            var old = new List<Tick>();
            
            ForEach(old.Add);
            old.ForEach(x => x.Remove());

            if (Info.Ticks != null)
            {
                foreach (TickInfo o in Info.Ticks)
                {
                    Children.Add(new Tick(o.Value, o.Text));
                }
            }
        }

        public Axis()
        {
            //this.RegisterAutoBinding(Refresh);
        }

        #region Major Gridline
        public class MajorGridline : Gridline
        {
            Tick _tick;
            public MajorGridline(Tick tick)
            {
                Color = Colors.LightGray;

                _tick = tick;
                Orientation = _tick.Parent is HorizontalAxis ? Orientation.Vertical : Orientation.Horizontal;
            }
            protected override Size ArrangeOverride(Rect bounds)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    bounds.Y = _tick.Center.Y;
                }
                else
                {
                    bounds.X = _tick.Center.X;
                }
                return base.ArrangeOverride(bounds);
            }
        }
        #endregion

        #region Ticks
        public void ForEach(Action<Tick> callback)
        {
            foreach (Tick tick in Children) callback(tick);
        }
        protected abstract Rect GetTickBounds(double visualValue, double width, double height);
        public class Tick : Label
        {
            public double Value { get; set; }

            MajorGridline? _line;
            public MajorGridline Line
            {
                get 
                { 
                    if (_line == null)
                    {
                        _line = new MajorGridline(this);
                    }
                    return _line; 
                }
            }
            public Point Center { get; set; }
            public Tick(double value = 0, string? text = null)
            {
                FontSize = 9.5;
                Padding = new Thickness(8, 4, 8, 4);

                Value = value;
                Text = text;
            }
            protected override void OnParentChanged()
            {
                this.GetParent<Axis>(ax => {

                    if ((Rotation = ax.Info.Rotation) != 0)
                        Padding = 8;

                    if (Text == null)
                        Text = ax.Info?.TickLabelCreator?.Invoke(Value);
                    
                    ax.GetParent<ChartView>(p => p.Area.CreateMajorGridLine(this));
                });
                base.OnParentChanged();
            }
            protected override Size ArrangeOverride(Rect bounds)
            {
                bounds = ((Axis)Parent).GetTickBounds(((Axis)Parent).GetVisualValue(Value), DesiredSize.Width, DesiredSize.Height);
                Center = bounds.Center;
                return base.ArrangeOverride(bounds);
            }
            public void Remove()
            {
                ((Axis)Parent).Remove(this);
                if (_line != null)
                {
                    ((Grid)_line.Parent).Remove(_line);
                }
            }
        }
        #endregion
    }

    public class HorizontalAxis : Axis
    {
        protected override Rect GetTickBounds(double visualValue, double width, double height)
        {
            return new Rect(visualValue - width / 2, 0, width, height);
        }

        public override double GetVisualValue(double value)
        {
            return Info.GetVisualValue(value);
        }
    }

    public class VerticalAxis : Axis
    {
        protected override Rect GetTickBounds(double visualValue, double width, double height)
        {
            return new Rect(this.DesiredSize.Width - width, visualValue - height / 2, width, height);
        }
        public override double GetVisualValue(double value)
        {
            return Info.GetVisualValue(value, true);
        }
    }

}
