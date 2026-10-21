// Copyright © 2022-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using EgonsoftHU.Extensions.Bcl;

namespace EgonsoftHU.Extensions.Logging
{
    /// <summary>
    /// Controls the log event property name for the source member provided by <see cref="CallerMemberNameAttribute"/>.
    /// </summary>
    [SuppressMessage("Style", "IDE0032")]
    public class SourceMemberNamingConfiguration
    {
        /// <summary>
        /// The default log event property name for the source member.
        /// </summary>
        public const string DefaultPropertyName = "SourceMember";

        private string currentPropertyName = DefaultPropertyName;

        private SourceMemberNamingConfiguration()
        {
        }

        /// <summary>
        /// Gets the current configuration for the log event property name for the source member.
        /// </summary>
        public static SourceMemberNamingConfiguration Current { get; } = new SourceMemberNamingConfiguration();

        /// <summary>
        /// Gets or sets the log event property name for the source member.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the property name is set to be <see langword="null"/>, <see cref="String.Empty"/> or consists only of white-space characters.
        /// </exception>
        public string PropertyName
        {
            get => currentPropertyName;
            set
            {
                value.ThrowIfNullOrWhiteSpace();
                currentPropertyName = value;
            }
        }
    }
}
