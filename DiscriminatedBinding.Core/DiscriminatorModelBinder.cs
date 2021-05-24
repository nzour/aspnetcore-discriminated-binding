using System;
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
        private readonly DiscriminatorAttribute _discriminator;
        private readonly IList<DiscriminatorCaseAttribute> _cases;
        private readonly IDiscriminatorReader _reader;

        public DiscriminatorModelBinder(
            DiscriminatorAttribute discriminator,
            IList<DiscriminatorCaseAttribute> cases,
            IDiscriminatorReader reader
        )
        {
            _discriminator = discriminator;
            _cases = cases;
            _reader = reader;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var property = await _reader.ReadDiscriminatorAsync(_discriminator.Property, bindingContext.HttpContext);

            if (null == property)
            {
                throw new CouldNotReadDiscriminatorException();
            }

            var resolvedType = Fn.CreateResolver(_cases)(property);

            if (null == resolvedType)
            {
                throw new UnresolvableDiscriminatorCaseException(property);
            }

            throw new NotImplementedException();
        }
    }
}