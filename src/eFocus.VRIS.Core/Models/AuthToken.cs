using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eFocus.VRIS.Core.Models
{
    public class AuthToken
    {
        public string AccessToken { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }
        public string AccessTokenType { get; set; }
        public string IdToken { get; set; }
        public string TenantId { get; set; }
    }
}
