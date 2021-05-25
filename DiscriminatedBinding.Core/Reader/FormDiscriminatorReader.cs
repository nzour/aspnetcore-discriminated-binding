using System.Linq;
using System.Threading.Tasks;
using DiscriminatedBinding.Core.Utility;
using Microsoft.AspNetCore.Http;

namespace DiscriminatedBinding.Core.Reader
{
    public sealed class FormDiscriminatorReader : IDiscriminatorReader
    {
        public Task<string?> ReadDiscriminatorAsync(string property, HttpContext context)
        {
            var form = context.Request.Form;

            return Task.FromResult(
                Fn.GenerateAllPropertyVariates(property)
                    .Select(it =>
                        form.TryGetValue(property, out var discriminatorValue)
                            ? discriminatorValue.ToString()
                            : null
                    )
                    .FirstOrDefault(it => null != it)
            );
        }
    }
}
