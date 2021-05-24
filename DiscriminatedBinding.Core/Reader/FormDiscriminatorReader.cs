using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DiscriminatedBinding.Core.Reader
{
    public sealed class FormDiscriminatorReader : IDiscriminatorReader
    {
        public Task<string?> ReadDiscriminatorAsync(string property, HttpContext context)
        {
            var form = context.Request.Form;

            return Task.FromResult(
                form.TryGetValue(property, out var discriminatorValue)
                    ? discriminatorValue.ToString()
                    : null
            );
        }
    }
}
