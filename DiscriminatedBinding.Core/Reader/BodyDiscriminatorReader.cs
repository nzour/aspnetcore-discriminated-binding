using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DiscriminatedBinding.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;

namespace DiscriminatedBinding.Core.Reader
{
    public sealed class BodyDiscriminatorReader : IDiscriminatorReader
    {
        private readonly IList<IInputFormatter> _formatters;
        private readonly IModelMetadataProvider _metadataProvider;
        private readonly bool _allowEmptyInputInBodyModelBinding;

        public BodyDiscriminatorReader(
            IList<IInputFormatter> formatters,
            IModelMetadataProvider metadataProvider,
            bool allowEmptyInputInBodyModelBinding
        )
        {
            _formatters = formatters;
            _metadataProvider = metadataProvider;
            _allowEmptyInputInBodyModelBinding = allowEmptyInputInBodyModelBinding;
        }

        public async Task<string?> ReadDiscriminatorAsync(string property, HttpContext context)
        {
            var inMemoryStreamCopy = new MemoryStream();

            // If we use FileBufferingReadStream (see context.Request.EnableBuffering()) https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.webutilities.filebufferingreadstream?view=aspnetcore-5.0
            // then Newtonsoft's input formatter always disposes it.
            //
            // If we will use default tream, then Newtonsoft's input formatter will create FileBufferingReadStream and then dispose.
            // But we need to read several times from Stream, so we need seakable stream (in case to rewind it)
            // That's why we need to copy default stream (actually is instance of HttpRequestStream) to any other seakable stream
            // Warning: maybe not memory safe - if body is too large.
            //
            // Maybe we should do this manipulation only when formatter is Newtonsoft's ? 🤔
            await context.Request.Body.CopyToAsync(inMemoryStreamCopy);
            await context.Request.Body.DisposeAsync();

            context.Request.Body = inMemoryStreamCopy;
            context.Request.Body.Position = 0;
            context.Response.RegisterForDisposeAsync(context.Request.Body);

            var formatterContext = new InputFormatterContext(
                httpContext: context,
                modelName: "object",
                modelState: new ModelStateDictionary(),
                metadata: _metadataProvider.GetMetadataForType(typeof(object)),
                readerFactory: (stream, encoding) => new HttpRequestStreamReader(stream, encoding),
                _allowEmptyInputInBodyModelBinding
            );

            var formatter = _formatters.FirstOrDefault(it => it.CanRead(formatterContext));

            if (null == formatter)
            {
                throw new NoInputFormatterFoundException();
            }

            var formattingResult = await formatter.ReadAsync(formatterContext);

            // rewinding stream
            context.Request.Body.Position = 0;

            return formattingResult.Model switch
            {
                // System.Text.Json
                JsonElement jsonText => HandleJsonTextResult(jsonText, property),

                // Newtonsoft.Json
                JObject jObject => HandleNewtonSoftJsonResult(jObject, property),

                // Others not supported yet 
                _ => null
            };
        }

        private static string? HandleJsonTextResult(JsonElement json, string property)
        {
            // fixme not working if property name defined in Pascal case and actual is camel case etc...

            if (json.TryGetProperty(property, out var value))
            {
                if (value.ValueKind == JsonValueKind.String)
                {
                    return value.ToString();
                }
            }

            return null;
        }

        private static string? HandleNewtonSoftJsonResult(JObject jObject, string property)
        {
            // fixme support different naming strategies
            return jObject.Value<string>(property);
        }
    }
}
