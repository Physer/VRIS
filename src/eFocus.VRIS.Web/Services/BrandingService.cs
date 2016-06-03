using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using eFocus.VRIS.Web.Models.Data;
using Organization = eFocus.VRIS.Core.Models.Branding.Organization;

namespace eFocus.VRIS.Web.Services
{
    public static class BrandingService
    {
        public static Organization GetOrganizationForEmails(IEnumerable<string> attendeeEmails)
        {
            var firstExternalEmail = attendeeEmails.FirstOrDefault(x => x.IndexOf("efocus", StringComparison.InvariantCultureIgnoreCase) == -1);
            if (!string.IsNullOrWhiteSpace(firstExternalEmail))
            {
                var domain = new MailAddress(firstExternalEmail);
                var organizationName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(domain.Host.Substring(0, domain.Host.IndexOf(".")));
                return new Organization
                {
                    Name = organizationName,
                    LogoUrl = GetOrganizationLogo(organizationName)
                };
            }
            return new Organization
            {
                Name = "eFocus",
                LogoUrl = GetOrganizationLogo("efocus")
            };
        }

        private static string GetOrganizationLogo(string organizationName)
        {
            using (var context = new CalendarContext())
            {
                var organization = context.Organizations.FirstOrDefault(x => x.Name.Equals(organizationName, StringComparison.InvariantCultureIgnoreCase));
                return organization != null ? organization.LogoUrl : string.Empty;
            }
        }

        public static void UpdateLogo(string organizationName, string logoUrl)
        {
            using (var context = new CalendarContext())
            {
                var organization = context.Organizations.FirstOrDefault(x => x.Name.Equals(organizationName, StringComparison.InvariantCultureIgnoreCase));
                if (organization != null && !string.IsNullOrWhiteSpace(logoUrl))
                {
                    organization.LogoUrl = logoUrl;
                    context.SaveChanges();
                }
            }
        }
    }
}