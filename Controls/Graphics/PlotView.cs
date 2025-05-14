using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Fig = Microsoft.Maui.Controls.Shapes.Path;
namespace VSmauiApp.Controls.Graphics
{
    public class Pen
    {
        public class Stroke 
        {
            public double X { get; set; }
            public double Y { get; set; }

            protected virtual string CommandName => this.GetType().Name;
            protected virtual object CommandArgs => $"{X},{Y}";
            public override string ToString() => $"{CommandName} {CommandArgs}";
        }
        class M : Stroke { }
        class L : Stroke { }
        class A : Stroke 
        {
            string arg;
            public A(string arg)
            {
                this.arg = arg;
            }
            protected override object CommandArgs => arg;
        }
        class H : Stroke 
        {
            protected override object CommandArgs => X;
        }
        class V : Stroke
        {
            protected override object CommandArgs => Y;
        }
        class Z : Stroke 
        {
            public override string ToString() => "Z";
        }

        LinkedList<Stroke> strokes = new LinkedList<Stroke>();
        double x, y;

        public Pen Add(Stroke stroke)
        {
            stroke.X = x;
            stroke.Y = y;
            strokes.AddLast(stroke);

            return this;
        }
        string data = string.Empty;
        public Pen Insert(object? value)
        {
            data += ' ';
            data += value;
            return this;
        }
        public Pen Insert(string command, object? args)
        {
            return Insert($"{command}{args}");
        }
        public Pen Insert(string command, double x, double y)
        {
            return Insert($"{command}{x},{y}");
        }
        public Pen MoveTo(double x, double y)
        {
            this.x = x;
            this.y = y;
            if (strokes.Count == 0)
                Add(new M());

            return this;
        }
        public Pen MoveTo(Point point) => MoveTo(point.X, point.Y);
        public Pen Offset(double dx, double dy) => MoveTo(x + dx, y + dy);

        public Pen Close() => Add(new Z());
        public Pen Line() => Add(new L());
        public Pen Line(double dx, double dy) => Offset(dx, dy).Line();
        public Pen HorizontalLine() => Add(new H());
        public Pen HorizontalLine(double dx) => Offset(dx, 0).HorizontalLine();
        public Pen VerticalLine() => Add(new V());
        public Pen VerticalLine(double dy) => Offset(0, dy).VerticalLine();
        /// <summary>
        /// Tạo một cung
        /// </summary>
        /// <param name="rx">Bán kính ngang</param>
        /// <param name="ry">Bán kính dọc</param>
        /// <param name="rotation">Góc quay (độ)</param>
        /// <param name="largeFlag">Cong lưng (0, 1)</param>
        /// <param name="sweepFlag">Cong bụng (0, 1)</param>
        /// <param name="x">Điểm ngang cuối</param>
        /// <param name="y">Điểm dọc cuối</param>
        /// <returns></returns>
        public Pen Arc(double rx, double ry, double rotation, double largeFlag, double sweepFlag, double x, double y)
        {
            return Add(new A($"{rx},{ry} {rotation} {largeFlag},{sweepFlag} {x},{y}"));
        }

        public Fig CreateFigure()
        {
            var d = string.Join(" ", strokes);
            var p = new Fig();
            var g = new PathGeometryConverter().ConvertFrom(d);
            if (g != null)
            {
                p.Data = (Geometry)g;
            }
            return p;
        }
    }
    public class Peek
    {
        static public double Size { get; set; } = 8;
        
        public double X { get; set; }
        public double Y { get; set; }
        public Point VisualPoint { get; set; }
        public Peek() { }
        public Peek(double x, double y) { X = x; Y = y; }
        public void Calculate(Axis ox,  Axis oy) 
        {
            VisualPoint = new Point(ox.GetVisualValue(X), oy.GetVisualValue(Y));
        }

        public double Left => VisualPoint.X - Size / 2;
        public double Top => VisualPoint.Y - Size / 2;

        public static Fig Square(Peek peek)
        {
            var d = Size;
            var pen = new Pen().MoveTo(peek.Left, peek.Top)
                .HorizontalLine(d)
                .VerticalLine(d)
                .HorizontalLine(-d);

            return pen.CreateFigure();
        }
        public static Fig Circle(Peek peek)
        {
            var d = Size / 2;
            var x = peek.Left;
            var y = peek.VisualPoint.Y;

            var pen = new Pen().MoveTo(x, y);
            pen.Arc(d, d, 0, 1, 0, x + Size, y);
            pen.Arc(d, d, 0, 1, 0, x, y);

            return pen.CreateFigure();
        }
    }
    public class PeekCollection : LinkedList<Peek>
    {
        public PeekCollection() 
        { 
        }
        public PointCollection ToPoints()
        {
            var pts = new PointCollection();
            ForEach(p => pts.Add(p.VisualPoint));

            return pts;
        }
        public void ForEach(Action<Peek> action)
        {
            foreach (var item in this)
            {
                action(item);
            }
        }

        public void Add(Peek item)
        {
            AddLast(item);
        }
    }
    public abstract class PlotView : Grid, IRefreshOnBindingChanged
    {
        public PlotView() 
        {
        }

        public ChartInfo Info
        {
            get => (ChartInfo)BindingContext;
            set => BindingContext = value;
        }

        public PeekCollection Peeks { get; set; } = new PeekCollection();
        protected void Calculate(Axis ox, Axis oy)
        {
            Peeks.ForEach(p => p.Calculate(ox, oy));
        }
        protected void Calculate()
        {
            this.GetParent<ChartView>(view => {
                view.Axis((ox, oy) => {
                    Calculate(ox, oy);
                });
            });
        }
        protected abstract void OnDraw();
        public void Draw()
        {
            this.Children.Clear();
            if (Peeks.Count == 0) return;

            OnDraw();
            SetPlotAttributes();

            OnDrawCompleted?.Invoke(this, EventArgs.Empty);
        }
        public PlotView Draw(double[]? y, double[]? x = null)
        {
            if (y != null)
            {
                if (x == null || x.Length == 0)
                {
                    x = new double[y.Length];
                    for (int i = 0; i < y.Length; i++)
                        x[i] = i;
                }
                Peeks = new PeekCollection();
                for (int i = 0; i < x.Length; i++)
                {
                    Peeks.Add(new Peek(x[i], y[i]));
                }
            }
            return this;
        }
        protected virtual void SetPlotAttributes()
        {
            ForEach(s => {
                s.Stroke = Info.Stroke;
                s.StrokeThickness = Info.StrokeThickness;
            });
        }

        public void ConnectPeeks(double lineWidth, Color lineColor)
        {
            Children.Add(new Polyline { 
                Points = Peeks.ToPoints(),
                Stroke = lineColor,
                StrokeThickness = lineWidth
            });
        }
        public void ConnectPeeks() => ConnectPeeks(Info.StrokeThickness, Info.Stroke);
        public void DrawPeeks(Color color, Func<Peek, Shape> createShape)
        {
            Peeks.ForEach(p => { 
                var s = createShape(p);
                if (s != null)
                {
                    Children.Add(s);
                    s.Fill = color;
                }
            });
        }
        public void DrawPeeks(Func<Peek, Shape> createShape) => DrawPeeks(Info.Stroke, createShape);

        public void ForEach(Action<Shape> callback)
        {
            foreach (Shape s in this.Children) callback(s);
        }
        public void Refresh()
        {
            Calculate();
            Draw();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            
            this.Binding<ChartInfo>(i => {
                Draw(i.Oy.Values, i.Ox.Values);
            });
        }
        public event EventHandler? OnDrawCompleted;
    }

    public class ScatterChart : PlotView
    {
        public Func<Peek, Shape>? PeekCreator { get; set; }
        public ScatterChart() : this(null, true) { }
        public ScatterChart(Func<Peek, Shape>? peekCreator, bool hasLine = false)
        {
            PeekCreator = peekCreator;
            LineVisible = hasLine;
        }

        public bool LineVisible
        {
            get; set;
        }

        protected override void OnDraw()
        {
            if (LineVisible)
            {
                this.ConnectPeeks();
            }
            if (PeekCreator != null)
            {
                Peeks.ForEach(p => {
                    var shape = PeekCreator(p);
                    if (shape != null)
                    {
                        Children.Add(shape);
                        shape.Fill = Info.Stroke;
                    }
                });
            }
        }
    }
    public class BarChart : PlotView
    {
        protected override void OnDraw()
        {
            this.GetParent<ChartView>(c => {
                double w = c.Ox.VisualSize / Peeks.Count / 2;
                Peeks.ForEach(p => {
                    var pen = new Pen().MoveTo(p.VisualPoint.X - w / 2, p.VisualPoint.Y)
                        .HorizontalLine(w)
                        .VerticalLine(c.Oy.VisualSize - p.VisualPoint.Y)
                        .HorizontalLine(-w).Close();

                    var bar = pen.CreateFigure();
                    bar.Fill = Info.Stroke;

                    Children.Add(bar);
                });
            });
        }
    }
}
