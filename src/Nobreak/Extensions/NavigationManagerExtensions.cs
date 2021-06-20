using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Web;

namespace Nobreak.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static void NavigateTo(this NavigationManager navManager, string uri, bool forceLoad = false, object routeValues = null)
        {
            if (routeValues is not null)
                uri += "?" + string.Join('&', routeValues.ConvertToDictionary()
                    .Select(kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value.ToString())}"));
            navManager.NavigateTo(uri, forceLoad);
        }
    }
}
