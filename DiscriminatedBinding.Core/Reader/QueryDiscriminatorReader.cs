using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DiscriminatedBinding.Core.Reader
{
    public sealed class QueryDiscriminatorReader : IDiscriminatorReader
    {
        public Task<string?> ReadDiscriminatorAsync(string property, HttpContext context)
        {
            var query = context.Request.Query;

            return Task.FromResult(
                query.TryGetValue(property, out var discriminatorValue)
                    ? discriminatorValue.ToString()
                    : null
            );
        }
    }
}
