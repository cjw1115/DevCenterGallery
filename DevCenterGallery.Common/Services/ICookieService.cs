using System.Threading.Tasks;

namespace DevCenterGallary.Common.Services
{
    public interface ICookieService
    {
        Task<string> GetDevCenterCookie();
    }
}