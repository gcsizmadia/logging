// Copyright © 2022-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using EgonsoftHU.Extensions.Bcl;

using Serilog.Core;
using Serilog.Events;

namespace EgonsoftHU.Extensions.Logging
{
    /// <summary>
    /// Applied during logging to add additional information to log events.
    /// </summary>
    public partial class PropertyBagEnricher : ILogEventEnricher
    {
        internal readonly Dictionary<string, LogEventPropertyCreationInfo> Properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBagEnricher"/> class.
        /// </summary>
        public PropertyBagEnricher()
        {
            Properties = new Dictionary<string, LogEventPropertyCreationInfo>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBagEnricher"/> class.
        /// </summary>
        /// <returns>A new instance of the <see cref="PropertyBagEnricher"/> class.</returns>
        public static PropertyBagEnricher Create()
        {
            return
#if LANGVERSION12_0_OR_GREATER
                []
#else
                new()
#endif
                ;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBagEnricher"/> class that
        /// contains the SourceMember property with the specified value of the <paramref name="sourceMember"/> parameter.
        /// </summary>
        /// <param name="sourceMember">The caller member name. By default, it is provided by the <see cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>A new instance of the <see cref="PropertyBagEnricher"/> class.</returns>
        public static PropertyBagEnricher CreateForSourceMember([CallerMemberName] string? sourceMember = null)
        {
            return new PropertyBagEnricher().Here(sourceMember);
        }

        /// <summary>
        /// Add a property that will be added to all log events enriched by this enricher.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="destructureObjects">
        /// Whether to destructure the value. See https://github.com/serilog/serilog/wiki/Structured-Data
        /// </param>
        /// <returns>The <see cref="PropertyBagEnricher"/> so that additional calls can be chained.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>, <see cref="String.Empty"/> or consists only of white-space characters.</exception>
        public PropertyBagEnricher Add(string propertyName, object? value, bool destructureObjects = false)
        {
            propertyName.ThrowIfNullOrWhiteSpace();

            if (!Properties.ContainsKey(propertyName))
            {
                Properties.Add(
                    propertyName,
                    new LogEventPropertyCreationInfo(propertyName, value, destructureObjects)
                );
            }

            return this;
        }

        /// <summary>
        /// Adds the elements of the specified collection to the <see cref="PropertyBagEnricher"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the values in <paramref name="properties"/>.</typeparam>
        /// <param name="properties">
        /// The collection whose elements should be added to the <see cref="PropertyBagEnricher"/>.<br/>
        /// The collection itself cannot be <see langword="null"/>, but it can contain key-value pairs that have a value of <see langword="null"/>.
        /// </param>
        /// <returns>The <see cref="PropertyBagEnricher"/> so that additional calls can be chained.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="properties"/> is <see langword="null"/>.</exception>
        public PropertyBagEnricher AddRange<TValue>(IEnumerable<KeyValuePair<string, TValue>> properties)
        {
            properties.ThrowIfNull();

            foreach (KeyValuePair<string, TValue> property in properties)
            {
                Add(property.Key, property.Value);
            }

            return this;
        }

        /// <summary>
        /// Adds the caller member name (i.e. where the log event occurs) as a property
        /// that will be added to all log events enriched by this enricher.
        /// </summary>
        /// <param name="sourceMember">The caller member name. By default, it is provided by the <see cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>The <see cref="PropertyBagEnricher"/> so that additional calls can be chained.</returns>
        /// <remarks>
        /// The log event property name is <c>SourceMember</c> by default.<br/>
        /// It can be customized by setting the <c>SourceMemberNamingConfiguration.Current.PropertyName</c> property.
        /// </remarks>
        public PropertyBagEnricher Here([CallerMemberName] string? sourceMember = null)
        {
            return Add(SourceMemberNamingConfiguration.Current.PropertyName, sourceMember);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">
        /// Either <paramref name="logEvent"/> or <paramref name="propertyFactory"/> is <see langword="null"/>.
        /// </exception>
        void ILogEventEnricher.Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.ThrowIfNull();
            propertyFactory.ThrowIfNull();

            foreach (LogEventPropertyCreationInfo property in Properties.Values)
            {
                logEvent.AddPropertyIfAbsent(
                    propertyFactory.CreateProperty(
                        property.PropertyName,
                        property.Value,
                        property.DestructureObjects
                    )
                );
            }
        }
    }
}
