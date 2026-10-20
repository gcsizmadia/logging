// Copyright © 2022-2024 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace EgonsoftHU.Extensions.Logging
{
    [SuppressMessage("Naming", "CA1710")]
    public partial class PropertyBagEnricher : IReadOnlyDictionary<string, object?>
    {
        bool IReadOnlyDictionary<string, object?>.ContainsKey(string key)
        {
            return Properties.ContainsKey(key);
        }

        bool IReadOnlyDictionary<string, object?>.TryGetValue(string key, out object? value)
        {
            if (Properties.TryGetValue(key, out LogEventPropertyCreationInfo? logEventPropertyCreationInfo))
            {
                value = logEventPropertyCreationInfo.Value;
                return true;
            }

            value = null;
            return false;
        }

        object? IReadOnlyDictionary<string, object?>.this[string key] => Properties[key].Value;

        IEnumerable<string> IReadOnlyDictionary<string, object?>.Keys => Properties.Keys;

        IEnumerable<object?> IReadOnlyDictionary<string, object?>.Values => Properties.Values.Select(property => property.Value);

        int IReadOnlyCollection<KeyValuePair<string, object?>>.Count => Properties.Count;

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, object?>>)Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Value)).GetEnumerator();
        }
    }
}
