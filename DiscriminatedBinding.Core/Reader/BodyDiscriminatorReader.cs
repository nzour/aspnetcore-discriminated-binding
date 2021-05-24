using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DiscriminatedBinding.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;

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
            context.Request.EnableBuffering();

            using var streamReader = new HttpRequestStreamReader(context.Request.Body, Encoding.Default);

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

            // rewinding the stream
            context.Request.Body.Position = 0;

            return formattingResult.Model switch
            {
                // System.Text.Json
                JsonElement jsonText => HandleJsonTextResult(jsonText, property),
                // todo: implement other formatter like NewtonSoftJson
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
    }
}
