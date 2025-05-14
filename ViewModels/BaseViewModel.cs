using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace VSmauiApp
{
    using Models;
    public abstract class BaseViewModel
    {
        static Dictionary<Type, object> _map = new Dictionary<Type, object>();
        static public T GetObject<T>(Type type)
        {
            _map.TryGetValue(type, out object? obj);
            if (obj == null)
            {
                obj = Activator.CreateInstance(type);
                _map.Add(type, obj);
            }
            return (T)obj;
        }

        #region PAGE ATTRIBUTES
        public string? Title { get; set; }
        //public TabbedBarViewModel? TabItems { get; set; }

        //string _activeBarItem = string.Empty;
        //public string ActiveBarItem
        //{
        //    get
        //    {
        //        if (_activeBarItem == string.Empty && TabItems != null)
        //        {
        //            _activeBarItem = TabItems[0].Route;
        //        }
        //        return _activeBarItem;
        //    }
        //    set
        //    {
        //        if (value != _activeBarItem)
        //        {
        //            _activeBarItem = value;
        //        }
        //    }
        //}
        #endregion

        #region FLYOUT
        static UserModel? _user;
        static public UserModel User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserModel {
                        Name = "Phan Đức Thọ",
                        Address = "40 Đông Quan, Cầu Giấy",
                    };
                } 
                return _user;
            }
            set => _user = value;
        }
        #endregion
    }
    public class ItemsViewModel : BaseViewModel
    {
        public virtual void BeginLoadData()
        {
        }
        public List<object>? ListItems
        {
            get
            {
                if (_listItems == null)
                {
                    _listItems = new List<object>();
                    BeginLoadData();
                }
                return _listItems;
            }
            set => _listItems = value;
        }
        List<object>? _listItems;

        protected virtual void OnActiveItemChanged(object? item) { }
        public object? ActiveItem
        {
            get => _activeItem;
            set
            {
                if (_activeItem != value)
                {
                    _activeItem = value;
                    OnActiveItemChanged(_activeItem);
                }
            }
        }
        object? _activeItem;
    }
}
