// Copyright © 2022-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;

using EgonsoftHU.Extensions.Bcl;

namespace EgonsoftHU.Extensions.Logging
{
    /// <summary>
    /// This class contains extension methods that are available for types derived from the <see cref="Exception"/> type.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Populates the <see cref="Exception.Data"/> dictionary with the set of log event properties.
        /// </summary>
        /// <typeparam name="TException">The type of the <see cref="Exception"/>.</typeparam>
        /// <param name="ex">The <see cref="Exception"/> into which the log event properties should be populated.</param>
        /// <param name="enricher">The log event enricher that contains the log event properties to populate.</param>
        /// <returns>
        /// The same exception instance on which this extension method was called and into which the log event properties have been populated.
        /// </returns>
        /// <exception cref="ArgumentNullException">Either <paramref name="ex"/> or <paramref name="enricher"/> is <see langword="null"/>.</exception>
        public static TException Populate<TException>(this TException ex, PropertyBagEnricher enricher)
            where TException : Exception
        {
            ex.ThrowIfNull();
            enricher.ThrowIfNull();

            foreach (PropertyBagEnricher.LogEventPropertyCreationInfo property in enricher.Properties.Values)
            {
                ex.Data[property.PropertyName] = property.Value;
            }

            return ex;
        }
    }
}
