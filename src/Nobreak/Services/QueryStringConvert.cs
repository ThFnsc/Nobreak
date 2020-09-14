using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Nobreak.Services
{
    public static class QueryStringConvert
    {
        public static T DeserializeObject<T>(string rawResponse) where T : class
        {
            var parsedQS = HttpUtility.ParseQueryString(rawResponse);
            var obj = Activator.CreateInstance(typeof(T)) as T;
            var parsingFuncions = new Dictionary<Type, Func<string, object>>
                {
                    {typeof(string), o=>o },
                    {typeof(int), o=>int.Parse(o) },
                    {typeof(long), o=>long.Parse(o) },
                    {typeof(float), o=>float.Parse(o) },
                    {typeof(double), o=>double.Parse(o) },
                    {typeof(decimal), o=>decimal.Parse(o) },
                    {typeof(bool), o=>bool.Parse(o) },
                    {typeof(DateTime), o=>DateTime.Parse(o) },
                };
            foreach (var prop in typeof(T).GetProperties())
                if (parsedQS.AllKeys.Contains(prop.Name))
                    prop.SetValue(obj, parsingFuncions[prop.PropertyType](parsedQS[prop.Name]));
            return obj;
        }
    }
}
