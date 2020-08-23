using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class ExceptionExtensions
    {
        public static string Detailed(this Exception exception) =>
            exception == null
            ? "null"
            : $"{exception.GetType().FullName}: {exception.Message}" +
            $"\n\nException data: {JsonConvert.SerializeObject((object)exception.Data ?? new { }, Formatting.Indented)}" +
            $"\n\nStack trace: {exception.StackTrace}" +
            $"\n\nInner exception: {exception.InnerException.Detailed()}";

        public static string DetailedSlack(this Exception exception) =>
            exception == null
            ? "null"
            : $"{exception.GetType().FullName}: {exception.Message}".ToSlackCodeLine() +
            $"\n\nException data: \n"
                + JsonConvert.SerializeObject((object)exception.Data ?? new { }, Formatting.Indented).ToSlackCodeBlock() +
            $"\n\nStack trace: \n" +
                exception.StackTrace.ToSlackQuote() +
            $"\n\nInner exception: {exception.InnerException.DetailedSlack()}";

        public static Exception AddData(this Exception exception, object key, object value)
        {
            exception.Data[key] = value;
            return exception;
        }
    }
}
