using System;
using System.Linq;
using DiscriminatedBinding.Core.Exceptions;
using DiscriminatedBinding.Core.Reader;
using DiscriminatedBinding.Core.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DiscriminatedBinding.Core
{
    public class DiscriminatorModelBinderProvider : IModelBinderProvider
    {
        private const string BodyBindingSource = "Body";
        private const string FormBindingSource = "Form";
        private const string QueryBindingSource = "Query";
        private const string HeaderBindingSource = "Header";
        private const string PathBindingSource = "Path";

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

            var reader = CreateDiscriminatorReader(context.BindingInfo.BindingSource);

            return null == reader
                ? null
                : new DiscriminatorModelBinder(discriminator, cases, reader);
        }

        private static IDiscriminatorReader? CreateDiscriminatorReader(BindingSource source)
        {
            return source.Id switch
            {
                BodyBindingSource => new BodyDiscriminatorReader(),
                FormBindingSource => new FormDiscriminatorReader(),
                QueryBindingSource => new QueryDiscriminatorReader(),
                HeaderBindingSource => new HeaderDiscriminatorReader(),
                PathBindingSource => new PathDiscriminatorReader(),
                _ => null,
            };
        }
    }
}