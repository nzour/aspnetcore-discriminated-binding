using System;
using System.Linq;
using DiscriminatedBinding.Core.Exceptions;
using DiscriminatedBinding.Core.Reader;
using DiscriminatedBinding.Core.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace DiscriminatedBinding.Core
{
    public class DiscriminatorModelBinderProvider : IModelBinderProvider
    {
        private const string BodyBindingSource = "Body";
        private const string FormBindingSource = "Form";
        private const string QueryBindingSource = "Query";
        private const string HeaderBindingSource = "Header";

        private readonly IOptions<MvcOptions> _mvcOptions;
        private readonly IPropertyNamingStrategy _propertyNamingStrategy;

        public DiscriminatorModelBinderProvider(IOptions<MvcOptions> mvcOptions, DiscriminatorBinderOptions options)
        {
            _mvcOptions = mvcOptions;
            _propertyNamingStrategy = options.PropertyNamingStrategy;
        }

        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var modelType = context.Metadata.ModelType;

            if (null == modelType)
            {
                // Returning null means go and try other IModelBinderProvider
                // Model type should be preset
                return null;
            }

            var modelAttributes = modelType.GetCustomAttributes(inherit: false);

            var discriminator = Fn.FindDiscriminator(modelAttributes);
            var cases = Fn.FilterDiscriminatorCases(modelAttributes).ToList();

            if (null == discriminator)
            {
                // If discriminator attribute not preset, then we do nothing
                return null;
            }

            if (!cases.Any())
            {
                throw new NoDiscriminatorCasesProvidedException(modelType);   
            }

            var reader = CreateDiscriminatorReader(_mvcOptions, context.MetadataProvider, context.BindingInfo.BindingSource);

            if (null == reader)
            {
                return null;
            }

            var factory = new ModelBinderFactory(context.MetadataProvider, _mvcOptions, context.Services);

            return new DiscriminatorModelBinder(factory, context.MetadataProvider, discriminator, cases, reader, _propertyNamingStrategy);
        }

        private static IDiscriminatorReader? CreateDiscriminatorReader(
            IOptions<MvcOptions> mvcOptions,
            IModelMetadataProvider metadataProvider,
            BindingSource source
        )
        {
            return source.Id switch
            {
                BodyBindingSource => new BodyDiscriminatorReader(
                    formatters: mvcOptions.Value.InputFormatters,
                    metadataProvider: metadataProvider,
                    mvcOptions.Value.AllowEmptyInputInBodyModelBinding
                ),
                FormBindingSource => new FormDiscriminatorReader(),
                QueryBindingSource => new QueryDiscriminatorReader(),
                HeaderBindingSource => new HeaderDiscriminatorReader(),
                _ => null,
            };
        }
    }
}