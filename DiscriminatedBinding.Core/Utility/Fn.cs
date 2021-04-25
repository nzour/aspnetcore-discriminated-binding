using System;
using System.Collections.Generic;
using System.Linq;
using DiscriminatedBinding.Core.Attributes;

namespace DiscriminatedBinding.Core.Utility
{
    internal static class Fn
    {
        public static Func<string, Type?> CreateResolver(IEnumerable<DiscriminatorCaseAttribute> cases)
        {
            var casesMap = cases.ToDictionary(it => it.Key, it => it.Implementation);

            return actual => casesMap.ContainsKey(actual) ? casesMap[actual] : null;
        }

        public static DiscriminatorAttribute? FindDiscriminator(IEnumerable<object> attributes)
        {
            return attributes.FirstOrDefault(it => it is DiscriminatorAttribute) as DiscriminatorAttribute;
        }

        public static IEnumerable<DiscriminatorCaseAttribute> FilterDiscriminatorCases(IEnumerable<object> attributes)
        {
            return attributes
                .Where(it => it is DiscriminatorCaseAttribute)
                .Cast<DiscriminatorCaseAttribute>();
        }
    }
}