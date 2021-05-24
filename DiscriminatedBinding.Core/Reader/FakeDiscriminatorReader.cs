using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DiscriminatedBinding.Core.Reader
{
    public class FakeDiscriminatorReader : IDiscriminatorReader
    {
        private readonly string? _expected;

        /// <param name="expected">The value that expected to be returned in method ReadDiscriminatorAsync</param>
        public FakeDiscriminatorReader(string? expected = null)
        {
            _expected = expected;
        }

        public Task<string?> ReadDiscriminatorAsync(string property, HttpContext context)
        {
            return Task.FromResult(_expected);
        }
    }
}
