using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSmauiApp.Controls.Graphics
{
    public enum Orientation
    {
        Horizontal, 
        Vertical,
    }
    public class Gridline : BoxView
    {
        public Gridline() 
        {
            Color = Colors.Black;
        }
        public Orientation Orientation
        {
            get => WidthRequest == 1 ? Orientation.Vertical : Orientation.Horizontal;
            set
            {
                if (value == Orientation.Vertical)
                {
                    WidthRequest = 1;
                    HorizontalOptions = LayoutOptions.Start;
                }
                else
                {
                    HeightRequest = 1;
                    VerticalOptions = LayoutOptions.Start;
                }
            }
        }
    }
}
