using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eFocus.VRIS.Core.Models.Branding;

namespace eFocus.VRIS.Web.Models.ViewModels
{
    public class OrganizationViewModel
    {
        public string OrganizationName { get; set; }
        public string LogoUrl { get; set; }
        public List<SelectListItem> DropdownItems { get; set; }
    }
}