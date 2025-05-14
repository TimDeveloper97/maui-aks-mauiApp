using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSmauiApp.Controls.Graphics
{
    public class ChartArea : Grid
    {

        Gridline[] _borders = new Gridline[] {
            new Gridline { Orientation = Orientation.Vertical },
            new Gridline { Orientation = Orientation.Horizontal },
            new Gridline { Orientation = Orientation.Vertical, HorizontalOptions = LayoutOptions.End },
            new Gridline { Orientation = Orientation.Horizontal, VerticalOptions = LayoutOptions.End },
        };
        Grid _majorContent = new Grid();
        Grid _plotsContent = new Grid();
        public ChartArea()
        {
            this.Add(_majorContent);
            this.Add(this._plotsContent);

            foreach (var line in _borders)
                Add(line);
        }

        public IList<IView> Plots => _plotsContent.Children;

        public void CreateMajorGridLine(Axis.Tick tick)
        {
            _majorContent.Add(tick.Line);
        }
        public Point GetVisualPoint(double x, double y)
        {
            this.GetParent<ChartView>(c => c.Axis((ox, oy) => {
                x = ox.GetVisualValue(x);
                y = oy.GetVisualValue(y);
            }));
            return new Point(x, y);
        }
    }
}
