using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DiscriminatedBinding.Core.Reader
{
    public interface IDiscriminatorReader
    {
        Task<string?> ReadDiscriminatorAsync(string property, HttpContext context);
    }
}
