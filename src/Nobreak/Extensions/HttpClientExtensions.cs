using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Nobreak.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<T> MakeRequest<T>(this HttpClient http, HttpRequest<T> httpRequest) where T : class
        {
            var req = new HttpRequestMessage
            {
                Method = httpRequest.Method,
                RequestUri = http.BaseAddress.SetPath(httpRequest.Path).AddQSFromDict(httpRequest.QueryString)
            };

            if (httpRequest.Body != null)
                req.Content = httpRequest.SendBodyType switch
                {
                    SendBodyTypes.Json => req.Content = new StringContent(httpRequest.Body.ToJSON(), Encoding.UTF8, "application/json"),
                    SendBodyTypes.URLEncoded => req.Content = new FormUrlEncodedContent(httpRequest.Body.ConvertToDictionary().Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value.ToString()))),
                    _ => throw new NotImplementedException(httpRequest.SendBodyType.ToString())
                };

            var res = await http.SendAsync(req);

            var resBody = await res.Content.ReadAsStringAsync();

            Exception Untreated() =>
                new HttpRequestException($"Erro no {req.Method} para '{req.RequestUri}': {res.StatusCode}")
                    .AddData("Request", httpRequest)
                    .AddData("ResponseBody", resBody);
            
            TParse Parse<TParse>() where TParse:class =>
                res.Content.Headers.ContentType.MediaType.ToLower() switch
                {
                    "application/json" => JsonConvert.DeserializeObject<TParse>(resBody),
                    "application/x-www-form-urlencoded" => QueryStringConvert.DeserializeObject<TParse>(resBody),
                    _ => typeof(TParse) == typeof(string) ? resBody as TParse: throw new NotImplementedException($"O mediatype {res.Content.Headers.ContentType} não é suportado")
                        .AddData("Request", httpRequest)
                        .AddData("ResponseBody", resBody),
                };

            if (!res.IsSuccessStatusCode)
                if (httpRequest.OnNotSuccessfullCode == null)
                    throw Untreated();
                else
                    return httpRequest.OnNotSuccessfullCode((res, resBody, Untreated, Parse<dynamic>));

            return Parse<T>();
        }
    }

    public class HttpRequest<T>
    {
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public string Path { get; set; } = string.Empty;
        public object Body { get; set; }
        public IDictionary<string, object> QueryString { get; set; }
        public SendBodyTypes SendBodyType { get; set; } = SendBodyTypes.Json;

        public Func<(HttpResponseMessage response, string body, Func<Exception> notHandled, Func<dynamic> parseBody), T> OnNotSuccessfullCode { get; set; }

        public HttpRequest()
        {
            QueryString = new Dictionary<string, object>();
        }
    }

    public enum SendBodyTypes
    {
        Json,
        URLEncoded
    }
}
