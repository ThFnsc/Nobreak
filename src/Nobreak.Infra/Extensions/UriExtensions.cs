using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace System
{
    public static class UriExtensions
    {
        public static Uri AddQSFromDict(this Uri uri, IDictionary<string, object> values)
        {
            var builder = new UriBuilder(uri);
            var qs = HttpUtility.ParseQueryString(uri.Query);
            foreach (var kvp in values)
                qs[kvp.Key] = kvp.Value?.ToString();
            builder.Query = qs.ToString();
            return builder.Uri;
        }

        public static Uri SetPath(this Uri uri, string path) =>
            new Uri(uri, path);
    }
}
