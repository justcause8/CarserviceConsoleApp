using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarserviceConsoleApp.Models
{
    public class ServicePart
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int PartId { get; set; }
        public Part Part { get; set; }
    }
}
