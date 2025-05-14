using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics.CodeAnalysis;
namespace VSmauiApp.Controls.Graphics;

#region OLD
//public enum PlotType
//{
//    Line,
//    Circle,
//    Triangle,
//    Rectangle,
//    Fill = 16,
//}
//public class PlotInfo
//{
//    public double[]? X { get; set; }
//    public double[]? Y { get; set; }
//    public Color Color { get; set; } = Colors.Blue;
//    public PlotType Type { get; set; } = PlotType.Line;
//    public double Radius { get; set; } = 2.5;
//    public double Thickness { get; set; } = 1;

//    public Point GetAt(int i)
//    {
//        return new Point(X == null ? i : X[i], Y[i]);
//    }

//    public List<Shape> GetShapes(PointCollection pts)
//    {
//        var lst = new List<Shape>();
//        if (Type == PlotType.Line)
//        {
//            lst.Add(new Polyline { Points = pts });
//        }
//        else
//        {
//            Func<Point, Shape> treangle = (p) => {
//                var r = Radius / 2;
//                var y = p.Y + r;
//                var d = r * Math.Sqrt(3);
//                return new Polygon
//                {
//                    Points = new PointCollection {
//                            new Point(p.X, p.Y - Radius),
//                            new Point(p.X - d, y),
//                            new Point(p.X + d, y)
//                        },
//                };
//            };
//            Func<Point, Shape> circle = (p) => {
//                var r = Radius * 2;
//                return new Ellipse {
//                    WidthRequest = r,
//                    HeightRequest = r,
//                    Margin = new Thickness(p.X - Radius, p.Y - Radius, 0, 0),
//                };
//            };
//            Func<Point, Shape> rect = (p) => {
//                var r = Radius * 2;
//                return new Rectangle {
//                    WidthRequest = r,
//                    HeightRequest = r,
//                    Margin = new Thickness(p.X - Radius, p.Y - Radius, 0, 0),
//                };
//            };
//            Func<Point, Shape> create = null;
//            switch (Type & ~PlotType.Fill)
//            {
//                case PlotType.Circle: create = circle; break;
//                case PlotType.Triangle: create = treangle; break;
//                case PlotType.Rectangle: create = rect; break;
//            }
//            foreach (var p in pts)
//            {
//                lst.Add(create(p));
//            }
//        }

//        foreach (var s in lst)
//        {
//            s.Stroke = Color;
//            s.StrokeThickness = Thickness;
//            if (Type.HasFlag(PlotType.Fill))
//            {
//                s.Fill = Color;
//            }
//        }
//        return lst;
//    }
//}
//public class Plot : ContentView
//{
//    public abstract class Axe
//    {
//        protected double min = double.MaxValue;
//        protected double max = double.MinValue;
//        double major = 0;
//        protected double scale;
//        protected double screen_size;
//        List<MajorInfo>? lines;
//        void calculate_major_grid()
//        {
//            if (major <= 0 || min == double.MaxValue)
//                return;

//            int n = (int)(min / major);
//            double m = n * major;
//            while (m > min) m -= major;
//            min = m;

//            lines = new List<MajorInfo>();
//            while (m < max)
//            {
//                lines.Add(new MajorInfo(m));
//                m += major;
//            }
//            lines.Add(new MajorInfo(max = m));
//        }
//        public void SetMajorGrid(double value)
//        {
//            major = value;
//            calculate_major_grid();
//        }
//        public void SetMajorCount(int count)
//        {
//            int r = (int)(max - min);
//            int[] t = new int[] { 1, 2, 5 };

//            while (true)
//            {
//                foreach (var a in t)
//                {
//                    if (a * count >= r)
//                    {
//                        SetMajorGrid(a);
//                        return;
//                    }
//                }
//                for (int i = 0; i < t.Length; i++)
//                    t[i] *= 10;
//            }
//        }
//        public void SetRange(double min, double max)
//        {
//            this.max = max;
//            this.min = min;
//            calculate_major_grid();
//        }
//        public double[] SetValues(IEnumerable<double> values)
//        {
//            var lst = new List<double>();
//            foreach (var value in values)
//            {
//                double d = (double)value;
//                lst.Add(d);

//                if (d < min) min = d;
//                if (d > max) max = d;
//            }
//            calculate_major_grid();
//            return lst.ToArray();
//        }
//        public void SetScreenSize(double size)
//        {
//            scale = (screen_size = size) / (max - min);
//            foreach (var m in MajorLines)
//            {
//                m.Position = GetScreenValue(m.Value);
//            }
//        }
//        public virtual double GetScreenValue(object value)
//        {
//            double d = (double)value;
//            return (d - min) * scale;
//        }
//        public List<MajorInfo> MajorLines
//        {
//            get
//            {
//                if (lines == null)
//                {
//                    lines = new List<MajorInfo> {
//                            new MajorInfo(min),
//                            new MajorInfo(max),
//                        };
//                }
//                return lines;
//            }
//        }
//    }

//    class VAxe : Axe
//    {
//        public override double GetScreenValue(object value)
//        {
//            return screen_size - base.GetScreenValue(value);
//        }
//    }
//    class HAxe : Axe
//    {
//    }
//    public class MajorInfo
//    {
//        public double Position { get; set; }
//        public double Value { get; private set; }

//        public MajorInfo(double v)
//        {
//            Value = v;
//            Caption = new Label {
//                Text = v.ToString(),
//                TextColor = Colors.Gray,
//                FontSize = 10,
//            };
//        }
//        public Label Caption { get; private set; }
//        public Line Line { get; private set; } = new Line
//        {
//            StrokeThickness = 1,
//            Stroke = Colors.LightGray,
//        };

//        public Line SetHorizontal(double w, double h)
//        {
//            Line.X2 = w;
//            Line.Y1 = Line.Y2 = Position;
//            Caption.VerticalTextAlignment = TextAlignment.Center;
//            Caption.HorizontalTextAlignment = TextAlignment.End;
//            return Line;
//        }
//        public Line SetVertical(double w, double h)
//        {
//            Line.Y2 = h;
//            Line.X1 = Line.X2 = Position;
//            Caption.HorizontalTextAlignment = TextAlignment.Center;
//            return Line;
//        }
//    }

//    Axe vax = new VAxe();
//    Axe hax = new HAxe();
//    Grid canvas = new Grid { Background = Colors.White };
//    Label caption = new Label {
//        TextColor = Colors.Black,
//        FontSize = 14,
//        VerticalOptions = LayoutOptions.Start,
//        HorizontalTextAlignment = TextAlignment.Center,
//        VerticalTextAlignment = TextAlignment.Center,
//    };
//    List<PlotInfo> bags = new List<PlotInfo>();

//    protected override void OnSizeAllocated(double width, double height)
//    {
//        base.OnSizeAllocated(width, height);
//        if (width < 0 || height < 0) return;

//        _content.Children.Clear();
//        var bound = 50;
//        var w = width - (bound << 1);
//        var h = height - (bound << 1);

//        hax.SetScreenSize(canvas.WidthRequest = w);
//        vax.SetScreenSize(canvas.HeightRequest = h);

//        caption.HeightRequest = bound;
//        _content.Add(caption);
//        _content.Add(canvas);

//        canvas.Children.Clear();

//        double x0 = bound, y0 = height - bound;
//        foreach (var i in hax.MajorLines)
//        {
//            var line = i.SetVertical(w, h);
//            canvas.Children.Add(line);
//            _content.Children.Add(i.Caption);

//            i.Caption.WidthRequest = bound;
//            i.Caption.Margin = new Thickness(i.Position + bound - i.Caption.Width / 2, y0 + (bound >> 2), 0, 0);
//        }
//        foreach (var i in vax.MajorLines)
//        {
//            var line = i.SetHorizontal(w, h);
//            canvas.Children.Add(line);
//            _content.Children.Add(i.Caption);

//            i.Caption.WidthRequest = bound - (bound >> 2);
//            i.Caption.HeightRequest = (bound >> 1);
//            i.Caption.Margin = new Thickness(0, i.Position + bound - i.Caption.Height / 2, 0, 0);
//        }

//        foreach (var bag in bags)
//        {
//            var pts = new PointCollection();
//            for (int i = 0; i < bag.Y.Length; i++)
//            {
//                var p = bag.GetAt(i);
//                p.X = hax.GetScreenValue(p.X);
//                p.Y = vax.GetScreenValue(p.Y);
//                pts.Add(p);
//            }

//            foreach (var s in bag.GetShapes(pts))
//                canvas.Children.Add(s);
//        }
//    }

//    public void SetMajorGridLines(double x, double y)
//    {
//        hax.SetMajorGrid(x);
//        vax.SetMajorGrid(y);
//    }
//    public void SetMajorGridCount(int x, int y)
//    {
//        hax.SetMajorCount(x);
//        vax.SetMajorCount(y);
//    }
//    public void Add(PlotInfo info)
//    {
//        if (info.X == null)
//        {
//            hax.SetRange(0, info.Y.Length);
//        }
//        else
//        {
//            hax.SetValues(info.X);
//        }
//        vax.SetValues(info.Y);
//        bags.Add(info);
//    }

//    Grid _content = new Grid();
//    public Plot()
//    {
//        this.Content = _content;
//    }

//    #region BINDABLE PROPERTIES

//    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
//        nameof(TextColor), typeof(Color), typeof(Plot), Colors.Gray);
//    public Color TextColor 
//    {
//        get => (Color)GetValue(TextColorProperty);
//        set => SetValue(TextColorProperty, value);
//    }

//    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
//        nameof(Title),
//        typeof(string),
//        typeof(Plot),
//        propertyChanged: (n, o, v) => { ((Plot)n).caption.Text = (string)v; });
//    public string Title
//    {
//        get => (string)GetValue(TitleProperty);
//        set => SetValue(TitleProperty, value);
//    }

//    #endregion
//}

#endregion

