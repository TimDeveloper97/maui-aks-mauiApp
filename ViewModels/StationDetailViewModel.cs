using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSmauiApp.Controls;
using VSmauiApp.Controls.Graphics;

namespace VSmauiApp
{
    public class MinuteAxisInfo : AxisInfo
    {
        public MinuteAxisInfo()
        {
            this.CreateTicks(0, 60, 5, 0.5);
            this.TickLabelCreator = v => $"{(int)v}";
        }
    }
    public class HourAxisInfo : AxisInfo
    {
        public HourAxisInfo()
        {
            this.CreateTicks(0, 24, 4, 0.5);
            this.TickLabelCreator = v => $"{(int)v}:00";
            this.Rotation = -45;
        }
    }
    public class DayAxisInfo : AxisInfo
    {
        public DateTime StartDate { get; set; }
        public DayAxisInfo(DateTime start, DateTime end)
        {
            StartDate = start;

            var d = (end - StartDate).TotalDays;
            if (d < 7)
            {
                CreateTicks(0, 7, 1, 1);
            }
            else if (d < 14)
            {
                CreateTicks(0, 14, 2, 0.5);
            }
            else if (d < 31)
            {
                CreateTicks(0, 31, 4, 0.5);
            }

            this.TickLabelCreator = v => $"{StartDate.AddDays(v):dd.MM}";
            this.Rotation = -45;
        }
    }

    public class PowerAxisInfo : AxisInfo
    {
        public PowerAxisInfo()
        {
            this.TickLabelCreator = v => $"{(int)v}";
        }
        protected override void OnValuesChanged()
        {
            double[] max = [2.5, 5, 10];
            int i = 0;
            while (true)
            {
                var a = max[i];
                if (a >= Max)
                {
                    CreateTicks(0, a, a / 5, a / 10);
                    break;
                }

                if (++i == max.Length)
                {
                    for (int k = 0; k < max.Length; ++k) max[k] *= 10;
                    i = 0;
                }
            }
            base.OnValuesChanged();
        }

        public PowerAxisInfo Demo(int n)
        {
            var y = new double[n];
            var rand = new Random();

            for (int i = 0; i < n; i++)
            {
                y[i] = rand.NextDouble() * 20;
            }

            this.Values = y;
            return this;
        }
    }
}

namespace VSmauiApp.ViewModels
{
    public class StationDetailFilter
    {
        char _mode = 'n';
        public char Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                }
            }
        }
        public DateTime Start { get; set; } = DateTime.Today;
        public DateTime End { get; set; } = DateTime.Today;

        public Task Load()
        {
            return Task.CompletedTask;
        }
    }
}

namespace VSmauiApp.ViewModels
{
    using LiveChartsCore;
    using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
    using LiveChartsCore.SkiaSharpView;
    using Models;
    using VSmauiApp.Controls.Graphics;

    public abstract class StationDetailViewModel : ItemsViewModel
    {
        public static Station? Current
        {
            get => _current;
            set
            {
                if (_current != value)
                {
                    _current = value;
                    CurrentChanged?.Invoke(_current);
                }
            }
        }

        static Station? _current;
        static public event Action<Station?>? CurrentChanged;

        public string? Address => _current?.Address;

        StationDetailFilter? _filter;
        public StationDetailFilter? Filter => _filter;

        public StationDetailViewModel()
        {
            ActiveItem = _current;
            _filter = _current?.StatusFilter;
        }
    }
    public abstract class StationChartViewModel : StationDetailViewModel
    {
        public ChartInfo Chart { get; set; } = new ChartInfo();
        public ISeries[] Series { get; set; }
    }
    public class StationRealTimeViewModel : StationChartViewModel
    {
        public StationRealTimeViewModel()
        {
            Title = "Trạng thái tức thời";
            Chart = new ChartInfo
            {
                Title = "Công suất tiêu thụ",
                Ox = new MinuteAxisInfo(),
                Oy = new PowerAxisInfo().Demo(60),
            };

            Series = [
                new LineSeries<double>
                {
                    Values = [2, 1, 3, 5, 3, 4, 6],
                    Fill = null,
                    GeometrySize = 20,
                    DataLabelsSize = 20,
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                },
            ];
        }
    }
    public class StationStatusViewModel : StationChartViewModel
    {
        public StationStatusViewModel()
        {
            var n = 24;
            Title = "Tra cứu";
            Chart = new ChartInfo
            {
                Title = "Thống kê công suất",
                Ox = new HourAxisInfo(),
                Oy = new PowerAxisInfo().Demo(n),
            };

            Series = [
                new ColumnSeries<double>
                {
                    Values = [ 20, 50, 40, 20, 40, 30, 50, 20, 50, 40 ],

                    // Defines the distance between every bars in the series
                    Padding = 0,

                    // Defines the max width a bar can have
                    MaxBarWidth = double.MaxValue
                }
            ];
        }
    }
    public class StationHistoryViewModel : StationDetailViewModel
    {
        public IEnumerable<object>? Items { get; set; }
    }
    public class StationWarningViewModel : StationHistoryViewModel
    {
        public StationWarningViewModel()
        {
            Title = "Lịch sử bất thường";
            Items = new List<object> {
                new { s = "Khong nhan dien duoc thiet bi", t = "14:06:15 10/03/2025" },
            };
        }
    }
    public class StationAlarmViewModel : StationHistoryViewModel
    {
        public StationAlarmViewModel()
        {
            Title = "Lịch sử cảnh báo";
            Items = new List<object> {
                new { s = "Mat ket noi", t = "22:15:03 15/03/2025" },
                new { s = "Qua ap", t = "14:06:15 10/03/2025" },
            };
        }
    }
}

namespace VSmauiApp.ViewModels
{
    public class StationSettingViewModel : StationDetailViewModel
    {

    }
}