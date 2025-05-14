using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSmauiApp.ViewModels
{
    using Models;
    internal class StationsViewModel : MainViewModel
    {
        protected override void OnActiveItemChanged(object? item)
        {
            var s = item as Station;
            StationDetailViewModel.Current = s;
            App.Request("//station-now");
        }
        public StationsViewModel() {
            Title = "Các trạm";
            ListItems = new List<object> {
                        new Station { ObjectId = "0001", Address = "40 Đông Quan", 
                            StatusFilter = new StationDetailFilter{ Mode = 'g', Start = DateTime.Today.AddDays(-1) } 
                        },
                        new Station { ObjectId = "0002", Address = "26/104 Lê Thanh Nghị",
                            StatusFilter = new StationDetailFilter{ Mode = 'n', Start = DateTime.Today.AddDays(-2) } 
                        },
                        new Station { ObjectId = "0003", Address = "12/36 Tạ Quang Bửu",
                            StatusFilter = new StationDetailFilter{ Mode = 't', Start = DateTime.Today.AddMonths(-6) } 
                        },
                    };
        }
    }
}
