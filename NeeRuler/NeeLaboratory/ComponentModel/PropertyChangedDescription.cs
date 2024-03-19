using System.ComponentModel;

namespace NeeLaboratory.ComponentModel
{
    public class PropertyChangedDescription
    {
        public PropertyChangedDescription()
        {
        }

        public PropertyChangedDescription(string[] relayProperties) : this(relayProperties, null)
        {
        }

        public PropertyChangedDescription(PropertyChangedEventHandler callback) : this(null, callback)
        {
        }

        public PropertyChangedDescription(string[]? relayProperties, PropertyChangedEventHandler? callback)
        {
            RelayProperties = relayProperties;
            Callback = callback;
        }

        public string[]? RelayProperties { get; }

        public PropertyChangedEventHandler? Callback { get; }

    }
}
