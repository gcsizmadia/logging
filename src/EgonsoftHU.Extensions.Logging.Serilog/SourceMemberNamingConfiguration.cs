// Copyright © 2022-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
#if NET10_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Threading;

using EgonsoftHU.Extensions.Bcl;

namespace EgonsoftHU.Extensions.Logging
{
    /// <summary>
    /// Configures the log event property name for the source member.
    /// </summary>
    public static class SourceMemberNamingConfiguration
    {
        /// <summary>
        /// The default log event property name for the source member.
        /// </summary>
        public const string DefaultPropertyName = "SourceMember";

        private static readonly
#if NET9_0_OR_GREATER
        Lock
#else
        object
#endif
        SyncRoot = new();

        private static bool isInitialized;

#if NET10_0_OR_GREATER
        [SuppressMessage("Style", "IDE0032:Use auto property", Justification = "<Pending>")]
#endif
        private static string? propertyName;

        /// <summary>
        /// Gets the configured log event property name.
        /// </summary>
        public static string PropertyName => Volatile.Read(ref propertyName) ?? DefaultPropertyName;

        /// <summary>
        /// Initializes the configuration. This method can only be called once.
        /// </summary>
        /// <param name="customPropertyName">The custom log event property name.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="customPropertyName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="customPropertyName"/> is empty or consists only of white-space characters.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the configuration has already been initialized.
        /// </exception>
        public static void Initialize(string customPropertyName)
        {
            customPropertyName.ThrowIfNullOrWhiteSpace();

            lock (SyncRoot)
            {
                if (isInitialized)
                {
                    throw new InvalidOperationException("Source member naming has already been initialized.");
                }

                Volatile.Write(ref propertyName, customPropertyName);
                isInitialized = true;
            }
        }
    }
}
