using System.Linq;
using System.Threading.Tasks;
using DiscriminatedBinding.Core.Utility;
using Microsoft.AspNetCore.Http;

namespace DiscriminatedBinding.Core.Reader
{
    public sealed class QueryDiscriminatorReader : IDiscriminatorReader
    {
        public Task<string?> ReadDiscriminatorAsync(string property, HttpContext context)
        {
            var query = context.Request.Query;

            return Task.FromResult(
                Fn.GenerateAllPropertyVariates(property)
                    .Select(it =>
                        query.TryGetValue(it, out var discriminatorValue)
                            ? discriminatorValue.ToString()
                            : null
                    )
                    .FirstOrDefault(it => null != it)
            );
        }
    }
}
