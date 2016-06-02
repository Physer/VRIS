using System.Collections.Generic;
using System.Web.Http;
using eFocus.VRIS.Core.Models.CalendarImport;

namespace eFocus.VRIS.Web.Controllers
{
    public class CalendarImportController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post([FromBody]IEnumerable<Room> rooms)
        {
            return Ok();
        }
    }
}
