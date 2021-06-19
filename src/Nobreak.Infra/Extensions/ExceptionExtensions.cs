namespace System
{
    public static class ExceptionExtensions
    {
        public static string Detailed(this Exception exception) =>
            exception == null
            ? "null"
            : $"{exception.GetType().FullName}: {exception.Message}" +
            $"\n\nException data: {((object) exception.Data ?? new { }).ToJson(true)}" +
            $"\n\nStack trace: {exception.StackTrace}" +
            $"\n\nInner exception: {exception.InnerException.Detailed()}";

        public static string DetailedSlack(this Exception exception) =>
            exception == null
            ? "null"
            : $"{exception.GetType().FullName}: {exception.Message}".ToSlackCodeLine() +
            $"\n\nException data: \n"
                + ((object) exception.Data ?? new { }).ToJson(true).ToSlackCodeBlock() +
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
