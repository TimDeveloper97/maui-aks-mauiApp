using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSmauiApp.Models
{

    public class Station
    {
        public ViewModels.StationDetailFilter StatusFilter { get; set; } = new ViewModels.StationDetailFilter();
        public string? ObjectId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
    }
}
