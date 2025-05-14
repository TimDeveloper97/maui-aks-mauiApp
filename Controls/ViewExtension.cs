using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSmauiApp.Controls
{
    public interface IRefreshOnBindingChanged
    {
        void Refresh();
    }
    public static class ViewExtension
    {
        /// <summary>
        /// Đăng ký sự kiện click
        /// </summary>
        /// <param name="view">không được chứa Frame</param>
        /// <param name="callback">hàm xử lý sự kiện</param>
        static public void RegisterClickEvent(this View view, Action callback)
        {
            var tapGestureRecognizer = new TapGestureRecognizer { 
                NumberOfTapsRequired = 1,
            };
            tapGestureRecognizer.Tapped += (s, e) => {
                callback();
            };
            view.GestureRecognizers.Add(tapGestureRecognizer);
        }
        static public void GetParent<T>(this View view, Action<T> callback)
            where T : View
        {
            View? p = view.Parent as View;
            while (p != null)
            {
                if (p is T)
                {
                    callback((T)p);
                    return;
                }

                p = p.Parent as View;
            }
        }

        static public void Binding<T>(this View view, Action<T> callback)
            where T: class
        {
            var context = view.BindingContext as T;
            if (context == null) return;

            var bindingType = typeof(T);
            var type = view.GetType();

            foreach (var p in bindingType.GetProperties())
            {
                try
                {
                    var v = p.GetValue(view.BindingContext);
                    var d = type.GetProperty(p.Name);
                    if (d != null)
                        d.SetValue(view, v, null);
                }
                catch (Exception ex)
                {
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            callback?.Invoke(context);
        }
    }
}
