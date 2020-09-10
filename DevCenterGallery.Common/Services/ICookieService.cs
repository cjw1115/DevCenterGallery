using System.Threading.Tasks;

namespace DevCenterGalley.Common.Services
{
    public interface ICookieService
    {
        Task<string> GetDevCenterCookie();
    }
}