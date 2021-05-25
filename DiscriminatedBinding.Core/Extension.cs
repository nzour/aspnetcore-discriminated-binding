using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DiscriminatedBinding.Core
{
    public static class Extension
    {
        public static MvcOptions AddDiscriminatedModelBindingProvider(this MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new DiscriminatorModelBinderProvider(new OptionsWrapper<MvcOptions>(options)));

            return options;
        }
    }
}
