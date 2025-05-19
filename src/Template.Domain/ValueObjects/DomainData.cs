using Template.Domain.ValueObjects;

namespace Template.Domain
{
    public static partial class DomainData
    {
        /// <summary>
        /// Represents the null values for the domain.
        /// </summary>
        public static partial class NullValues
        {
            /// <summary>
            /// Gets the name of the key for <see cref="TKey"/> type.
            /// </summary>
            /// <typeparam name="TKey">The type of the key.</typeparam>
            /// <returns>The name of the key.</returns>
            public static string GetKeyName<TKey>()
                where TKey : struct, IIdentity<EntityId<TKey>, TKey>, IEquatable<TKey>
            {
                var type = typeof(TKey);
                return type.FullName ?? type.Name;
            }

            /// <summary>
            /// Gets a <see cref="string"/> representation of the <see langword="null"/> value for the <see cref="TKey"/> type.
            /// </summary>
            /// <typeparam name="TKey">The type of the key.</typeparam>
            /// <returns>A <see cref="string"/> representation of the <see langword="null"/> value.</returns>
            public static string GetNullValue<TKey>()
                where TKey : struct, IIdentity<EntityId<TKey>, TKey>, IEquatable<TKey>
            {
                return typeof(TKey).IsValueType ? $"default(GetKeyName<TKey>())" : "null";
            }
        }
    }
}
