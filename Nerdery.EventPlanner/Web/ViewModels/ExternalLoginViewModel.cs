using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ViewModels
{
    /// <summary>
    /// This view model is used for loging in user's via OAuth. A Facebook user, for example.
    /// </summary>
    public class ExternalLoginViewModel
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}