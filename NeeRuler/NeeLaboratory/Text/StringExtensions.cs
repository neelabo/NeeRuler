using System;
using System.Text;

namespace NeeLaboratory.Text
{
    public static class StringExtensions
    {
        private enum UnescapeState
        {
            Unescaped,
            Escaped
        }

        // from https://stackoverflow.com/questions/40347168/how-to-parse-an-escape-sequence
        public static string Unescape(this string s)
        {
            var sb = new StringBuilder(s.Length + 2);
            var state = UnescapeState.Unescaped;

            foreach (var ch in s)
            {
                switch (state)
                {
                    case UnescapeState.Escaped:
                        switch (ch)
                        {
                            case 't':
                                sb.Append('\t');
                                break;
                            case 'n':
                                sb.Append('\n');
                                break;
                            case 'r':
                                sb.Append('\r');
                                break;

                            case '\\':
                            case '\"':
                                sb.Append(ch);
                                break;

                            default:
                                //throw new Exception("Unrecognized escape sequence '\\" + ch + "'");
                                sb.Append('\\');
                                sb.Append(ch);
                                break;
                        }
                        state = UnescapeState.Unescaped;
                        break;

                    case UnescapeState.Unescaped:
                        if (ch == '\\')
                        {
                            state = UnescapeState.Escaped;
                        }
                        else
                        {
                            sb.Append(ch);
                        }
                        break;
                }
            }

            if (state == UnescapeState.Escaped)
            {
                //throw new Exception("Unterminated escape sequence");
                sb.Append('\\');
            }

            return sb.ToString();
        }

        public static string TrimPostfix(this string s, string trim)
        {
            var index = s.LastIndexOf(trim, StringComparison.Ordinal);
            if (index != s.Length - trim.Length) return s;
            return s.Substring(0, index);
        }

        public static string EnsurePostfix(this string s, string postfix)
        {
            var index = s.LastIndexOf(postfix, StringComparison.Ordinal);
            if (index == s.Length - postfix.Length) return s;
            return s + postfix;
        }
    }
}
