using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst.Controls.SVG;

using VS = Vst.Controls.SVG;

namespace VSmauiApp.Controls
{
    public class SvgCanvas : AbsoluteLayout, VS.IAddChild
    {
        public Rect ViewBox
        {
            get => new Rect(0, 0, WidthRequest, HeightRequest);
            set
            {
                WidthRequest = value.Width;
                HeightRequest = value.Height;
            }
        }
        public void AppendChild(object child) => base.Add((IView)child);

        protected void SetAttribute(BindableProperty property, object value)
        {
            foreach (View e in Children)
            {
                if (e is Shape)
                {
                    e.SetValue(property, value);
                }
                else if (e is SvgCanvas) 
                {
                    ((SvgCanvas)e).SetAttribute(property, value);
                }
            }
        }

        public SvgCanvas()
        {
            this.ChildAdded += (s, e) => {
                e.Element.SetValue(Shape.FillProperty, Fill);
                e.Element.SetValue(Shape.StrokeProperty, Stroke);
            };
        }

        public Color Fill { get => (Color)GetValue(FillProperty); set => SetValue(FillProperty, value); }
        static public readonly BindableProperty FillProperty = BindableProperty.Create(nameof(Fill), typeof(Color), typeof(SvgIcon),
            propertyChanged: (s, o, n) =>
            {
                ((SvgCanvas)s).SetAttribute(Shape.FillProperty, n);
            });

        public Color Stroke { get => (Color)GetValue(StrokeProperty); set => SetValue(StrokeProperty, value); }
        static public readonly BindableProperty StrokeProperty = BindableProperty.Create(nameof(Fill), typeof(Color), typeof(SvgIcon),
            propertyChanged: (s, o, n) =>
            {
                ((SvgCanvas)s).SetAttribute(Shape.StrokeProperty, n);
            });
    }
    public class SvgIcon : SvgCanvas
    {
        static public VS.ElementCollection Resource { get; private set; } = new VS.ElementCollection();
        static public void Register(string source)
        {
            using var resource = FileSystem.Current.OpenAppPackageFileAsync($"{source}.json").Result;
            Resource.Add(source, VS.Element.Parse(resource));
            //Task.Run(async () => {
            //});
        }

        static readonly ShapeCreator _shapes = new ShapeCreator {
            { "svg", typeof(SvgCanvas) },
            { "g", typeof(SvgCanvas) },
            { "line", typeof(Line) },
            { "path", typeof(Microsoft.Maui.Controls.Shapes.Path) },
            { "rect", typeof(Rectangle) },
            { "circle", typeof(Ellipse) },
            { "ellipse", typeof(Ellipse) },
        };

        static readonly AttributeConverter _attributes = new AttributeConverter {
            { typeof(ColorAttribute), v => Color.Parse((string)v) },
            { typeof(PointsAttribute), GetPoints },
            { typeof(DataPathAttribute), GetPathData },
            { typeof(ViewBoxAttribute), GetViewBox },
        };

        static object GetViewBox(object value)
        {
            var v = (double[])value;
            return new Rect(0, 0, v[2], v[3]);
        }
        static object GetPathData(object value)
        {
            var geo = new PathGeometryConverter();

            return geo.ConvertFromInvariantString((string)value) ?? geo;
        }
        static object GetPoints(object value)
        {
            var pts = new PointCollection();
            var v = (double[])value;

            int i = 0;
            while (i < v.Length)
            {
                var x = v[i++];
                var y = v[i++];
                pts.Add(new Point(x, y));
            }
            return pts;
        }
        public SvgIcon()
        {
            WidthRequest = HeightRequest = 16;
        }
        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        protected virtual void OnSourceChanged(string key)
        {
            var k = key.ToLower();
            if (Resource.TryGetValue(k, out var element))
            {
                LoadElement(element);
            }
        }
        protected virtual void LoadElement(VS.Element element)
        {
            try
            {
                this.Children.Clear();
                element.Attributes.GetAttribute<ViewBoxAttribute>("viewBox", a => {
                        var w = a.Width;
                        if (Math.Abs(w - WidthRequest) > 1)
                        {
                            element.ScaleTransform(WidthRequest / w);
                        }
                    });

                Dispatcher.Dispatch(() => {
                    element.Render(_shapes, _attributes, this);
                    SetAttribute(Shape.FillProperty, Fill);
                    SetAttribute(Shape.StrokeProperty, Stroke);
                });
            }
            catch
            {
            }
        }
        static public readonly BindableProperty SourceProperty = 
            BindableProperty.Create(
                nameof(Source), typeof(string), typeof(SvgIcon),
                defaultValue: string.Empty,
                propertyChanged: (s, o, n) => ((SvgIcon)s).OnSourceChanged((string)n));
    }
}
