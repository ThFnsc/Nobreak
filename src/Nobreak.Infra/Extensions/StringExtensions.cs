using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        public static string CleanCarriageReturns(this string input) =>
            string.IsNullOrWhiteSpace(input)
                ? input
                : string.Join("", input.Where(c => c != '\r'));

        public static string RemoveHTMLTags(this string input) =>
            Regex.Replace(input, "<.*?>", string.Empty);

        public static string CapitalizeFirstLetter(this string input) =>
            string.IsNullOrWhiteSpace(input)
                ? input
                : char.ToUpperInvariant(input[0]) + input.Substring(1);

        public static string ToCamelCase(this string input) =>
            char.ToLowerInvariant(input[0]) + input.Substring(1);

        public static string CapitalizeEveryFirstLetter(this string input) =>
            string.Join(' ', input.Split(' ').Select(p => p.CapitalizeFirstLetter()));

        public static string FirstWord(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            var space = input.IndexOf(' ');
            return space == -1 ? input : input.Substring(0, space);
        }

        public static string Repeat(this string input, int times, string separator = "")
        {
            var output = new StringBuilder();
            for (int i = 0; i < times; i++)
            {
                output.Append(input);
                if (i != times - 1)
                    output.Append(separator);
            }
            return output.ToString();
        }

        public static string BrainDamage(this string input)
        {
            var random = new Random();
            return new string(input.ToCharArray().Select(c=>random.NextDouble()>.5?char.ToLowerInvariant(c):char.ToUpperInvariant(c)).ToArray());
        }

        public static string ToSlackQuote(this string input) =>
            string.IsNullOrEmpty(input)
                ? string.Empty
                : string.Join("\n", input.SplitOnLineBreaks().Select(line => $">_{line}_"));

        public static string ToSlackCodeLine(this string input) =>
            string.IsNullOrEmpty(input)
                ? string.Empty
                : string.Join("\n", input.SplitOnLineBreaks().Select(line => $"`{line}`"));

        public static string ToSlackCodeBlock(this string input) =>
            string.IsNullOrEmpty(input)
                ? string.Empty
                : $"```{input}```";

        public static int CountWords(this string input) =>
            string.IsNullOrWhiteSpace(input)
            ? -1
            : input.Split(' ').Count(ss => ss.Length > 0);

        public static string[] SplitOnLineBreaks(this string input) =>
            input.Split(new string[] { "\r\n", "\r", "\n" },StringSplitOptions.None);

        public static string LastXChars(this string input, int qtd) =>
            input == null ? null : (input.Length < qtd ? input : input.Substring(input.Length - qtd, qtd));

        public static string FirstXChars(this string input, int qtd, string ifExceeds = null) =>
            input == null ? null : (input.Length < qtd ? input : $"{input.Substring(0, qtd)}{ifExceeds ?? string.Empty}");

        public static byte[] ToByteArray(this string input) =>
            Encoding.UTF8.GetBytes(input);

        public static T AsJson<T>(this string input) =>
            JsonSerializer.Deserialize<T>(input, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}