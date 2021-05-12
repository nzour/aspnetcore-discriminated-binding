using System;
using System.Collections.Generic;
using System.Linq;
using DiscriminatedBinding.Core.Attributes;
using DiscriminatedBinding.Core.Exceptions;
using DiscriminatedBinding.Core.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace DiscriminatedBinding.Core
{
    // todo find out the way to determine decorated binder and inject it into ctor.
    public class DiscriminatorModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
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
                // Returning null means go and try other IModelBinderProvider
                // If discriminator attribute not preset, then we do nothing
                return null;
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