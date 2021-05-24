using System.Collections.Generic;
using System.Threading.Tasks;
using DiscriminatedBinding.Core.Attributes;
using DiscriminatedBinding.Core.Exceptions;
using DiscriminatedBinding.Core.Reader;
using DiscriminatedBinding.Core.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DiscriminatedBinding.Core
{
    public class DiscriminatorModelBinder : IModelBinder
    {
        private readonly IModelBinderFactory _factory;
        private readonly IModelMetadataProvider _metadataProvider;
        private readonly DiscriminatorAttribute _discriminator;
        private readonly IList<DiscriminatorCaseAttribute> _cases;
        private readonly IDiscriminatorReader _reader;

        public DiscriminatorModelBinder(
            IModelBinderFactory factory,
            IModelMetadataProvider metadataProvider,
            DiscriminatorAttribute discriminator,
            IList<DiscriminatorCaseAttribute> cases,
            IDiscriminatorReader reader
        )
        {
            _factory = factory;
            _metadataProvider = metadataProvider;
            _discriminator = discriminator;
            _cases = cases;
            _reader = reader;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var actualDiscriminatorValue = await _reader.ReadDiscriminatorAsync(_discriminator.Property, bindingContext.HttpContext);

            if (null == actualDiscriminatorValue)
            {
                throw new CouldNotReadDiscriminatorException();
            }

            var resolvedType = Fn.CreateResolver(_cases)(actualDiscriminatorValue);

            if (null == resolvedType)
            {
                throw new UnresolvableDiscriminatorCaseException(actualDiscriminatorValue);
            }

            var metadataForResolvedType = _metadataProvider.GetMetadataForType(resolvedType);

            var modelBinderForResolvedType = _factory.CreateBinder(new ModelBinderFactoryContext
            {
                Metadata = metadataForResolvedType,
                BindingInfo = new BindingInfo
                {
                    BindingSource = bindingContext.BindingSource,
                    BinderModelName = bindingContext.BinderModelName
                }
            });

            if (null == modelBinderForResolvedType)
            {
                throw new CouldNotResolveBinderForTypeException(resolvedType);
            }

            bindingContext.ModelMetadata = metadataForResolvedType;

            await modelBinderForResolvedType.BindModelAsync(bindingContext);
        }
    }
}