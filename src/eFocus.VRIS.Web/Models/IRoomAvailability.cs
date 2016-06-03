using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eFocus.VRIS.Web.Models
{
    public interface IRoomAvailability
    {
        bool Available { get; }
        DateTime StartUtc { get; set; }
        DateTime EndUtc { get; set; }
    }
}
