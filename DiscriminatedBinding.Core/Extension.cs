using System;
using DiscriminatedBinding.Core.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DiscriminatedBinding.Core
{
    public class DiscriminatorBinderOptions
    {
        public readonly IPropertyNamingStrategy PropertyNamingStrategy = new CamelCaseNamingStrategy();
    }

    public static class Extension
    {
        public static MvcOptions AddDiscriminatedModelBindingProvider(this MvcOptions options, Action<DiscriminatorBinderOptions>? binderOptions = null)
        {
            var binderOptionsInstance = new DiscriminatorBinderOptions();
            binderOptions?.Invoke(binderOptionsInstance);

            options.ModelBinderProviders.Insert(0, new DiscriminatorModelBinderProvider(
                new OptionsWrapper<MvcOptions>(options),
                binderOptionsInstance
            ));

            return options;
        }
    }
}
