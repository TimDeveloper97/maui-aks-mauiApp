using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst.Controls.SVG
{
    public static class ShapeExtension
    {
        static public void SetAttribute(this Layout layout, BindableProperty property, object value)
        {
            foreach (View e in layout.Children)
            {
                if (e is Layout)
                {
                    SetAttribute((Layout)e, property, value);
                }
                else
                {
                    e.SetValue(property, value);
                }
            }
        }
    }
}
