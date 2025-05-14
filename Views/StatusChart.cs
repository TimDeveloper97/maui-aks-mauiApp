using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSmauiApp.Controls.Graphics;

namespace VSmauiApp.Views
{
    internal class StationChart : ChartView
    {
    }

    internal class SummaryChart : BarChart { }
    internal class StatusChart : ScatterChart
    {
        public StatusChart() 
        {
        }
    }
}
