// Copyright © 2022-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

namespace EgonsoftHU.Extensions.Logging
{
    public partial class PropertyBagEnricher
    {
        internal sealed record LogEventPropertyCreationInfo(string PropertyName, object? Value, bool DestructureObjects);
    }
}
