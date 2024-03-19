using System.Collections.Generic;
using System.Globalization;

namespace NeeLaboratory.Resources
{
    /// <summary>
    /// テキストリソース
    /// </summary>
    public class TextResourceSet
    {
        private readonly CultureInfo _culture;
        private readonly Dictionary<string, string> _map;


        public TextResourceSet()
        {
            _culture = CultureInfo.InvariantCulture;
            _map = new();
        }

        public TextResourceSet(CultureInfo culture, Dictionary<string, string> map)
        {
            _culture = culture;
            _map = map;
        }


        public CultureInfo Culture => _culture;

        public Dictionary<string, string> Map => _map;

        public bool IsValid => !_culture.Equals(CultureInfo.InvariantCulture);


        public string? this[string name]
        {
            get { return GetString(name); }
        }

        public string? GetString(string name)
        {
            return _map.TryGetValue(name, out var value) ? value : null;
        }
    }
}
