using Humanizer;

namespace DiscriminatedBinding.Core.Utility
{
    public interface IPropertyNamingStrategy
    {
        string ConvertPropertyName(string propertyName);
    }

    public sealed class IdNamingStrategy : IPropertyNamingStrategy
    {
        public string ConvertPropertyName(string propertyName)
        {
            return propertyName;
        }
    }

    public sealed class CamelCaseNamingStrategy : IPropertyNamingStrategy
    {
        public string ConvertPropertyName(string propertyName)
        {
            return propertyName.Camelize();
        }
    }

    public sealed class UnderscoredNamingStrategy : IPropertyNamingStrategy
    {
        public string ConvertPropertyName(string propertyName)
        {
            return propertyName.Underscore();
        }
    }

    public sealed class PascalCaseNamingStrategy : IPropertyNamingStrategy
    {
        public string ConvertPropertyName(string propertyName)
        {
            return propertyName.Pascalize();
        }
    }
}