using System;
using System.Collections.Generic;
using System.Linq;
using DiscriminatedBinding.Core.Attributes;
using Humanizer;

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

        public static IEnumerable<string> GenerateAllPropertyVariates(string property)
        {
            yield return property;

            var trimmed = property.Trim();

            yield return trimmed;
            yield return trimmed.Camelize();
            yield return trimmed.Pascalize();
            yield return trimmed.Underscore();
        }   
    }
}