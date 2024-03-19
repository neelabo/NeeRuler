using NeeLaboratory.Text;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace NeeLaboratory.Resources
{
    /// <summary>
    /// テキストリソース生成
    /// </summary>
    public class TextResourceFactory
    {
        private static readonly char[] _tokenSeparator = new char[] { '=' };

        private readonly LanguageResource _languageResource;

        public TextResourceFactory(LanguageResource languageResource)
        {
            _languageResource = languageResource;
        }

        public TextResourceSet Load(CultureInfo culture)
        {
            var culture0 = _languageResource.DefaultCulture;
            var culture1 = culture;

            var res0 = LoadResText(culture0);
            var res = res0;

            if (!culture1.Equals(culture0))
            {
                var res1 = LoadResText(culture1);
                res = res1.Concat(res0).GroupBy(e => e.Key, (key, keyValues) => keyValues.First());
            }

            return new TextResourceSet(culture1, res.ToDictionary(e => e.Key, e => e.Value));
        }

        private IEnumerable<KeyValuePair<string, string>> LoadResText(CultureInfo culture)
        {
            return LoadResText(_languageResource.CreateFileSource(culture));
        }

        private static IEnumerable<KeyValuePair<string, string>> LoadResText(IFileSource fileSource)
        {
            Stream stream;
            try
            {
                stream = fileSource.Open();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new List<KeyValuePair<string, string>>();
            }

            try
            {
                using var reader = new StreamReader(stream, Encoding.UTF8);

                return ReadLines(reader)
                    .Select(e => DeserializeResText(e))
                    .OfType<KeyValuePair<string, string>>()
                    .ToList();
            }
            finally
            {
                stream.Dispose();
            }
        }

        private static IEnumerable<string> ReadLines(StreamReader reader)
        {
            while (true)
            {
                var s = reader.ReadLine();
                if (s is null) yield break;
                yield return s;
            }
        }

        private static KeyValuePair<string, string>? DeserializeResText(string s)
        {
            s = s.Trim();
            if (string.IsNullOrEmpty(s) || s[0] == ';' || s[0] == '#')
            {
                return null;
            }
            var tokens = s.Split(_tokenSeparator, 2);
            if (tokens.Length != 2)
            {
                Debug.WriteLine($"ResText: FormatException");
                return null;
            }
            var key = tokens[0].Trim();
            var body = tokens[1].Trim().Unescape();
            if (string.IsNullOrEmpty(body))
            {
                return null;
            }
            return new(key, body);
        }
    }
}
