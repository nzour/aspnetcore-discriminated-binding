using System;
using System.Collections.Generic;
using System.Linq;
using DiscriminatedBinding.Core.Attributes;
using DiscriminatedBinding.Core.Exceptions;
using DiscriminatedBinding.Core.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DiscriminatedBinding.Core
{
    // todo find out the way to determine decorated binder and inject in into ctor.
    public class DiscriminatorModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var modelAttributes = context.Metadata.ModelType.GetCustomAttributes(inherit: false);

            var discriminator = Fn.FindDiscriminator(modelAttributes);
            var cases = Fn.FilterDiscriminatorCases(modelAttributes).ToList();

            if (null == discriminator)
            {
                throw new DiscriminatorException("No implemented yet"); // todo: exception
            }

            if (!cases.Any())
            {
                throw new Exception("No implemented yet"); // todo: exception   
            }

            // todo: here we should read value from request and find out what implementation from case we should use

            // fixme
            string? discriminatorValue = ""; 

            var implementationType = Fn.CreateResolver(cases)(discriminatorValue);

            return new DiscriminatorModelBinder();
        }
    }
}