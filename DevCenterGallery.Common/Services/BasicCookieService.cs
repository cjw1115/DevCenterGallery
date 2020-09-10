using DevCenterGalley.Common.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DevCenterGallery.Common.Services
{
    public abstract class BasicCookieService : ICookieService
    {
        public abstract Task<string> GetDevCenterCookie();

        protected Tuple<string, string> GetAccountInfo()
        {
            using (var configStream = File.Open(Path.Combine(Directory.GetCurrentDirectory(), "DevCenterConfig.json"), FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[configStream.Length];
                configStream.Read(buffer, 0, buffer.Length);
                var configJson = JsonDocument.Parse(Encoding.UTF8.GetString(buffer));
                string usernmae = string.Empty;
                string password = string.Empty;

                JsonElement userinfoPair = new JsonElement();
                foreach (var item in configJson.RootElement.EnumerateObject())
                {
                    if (item.Name == this.GetType().Name)
                    {
                        userinfoPair = item.Value;
                    }
                }
                foreach (var item in userinfoPair.EnumerateObject())
                {
                    if (item.Name == "username")
                    {
                        usernmae = item.Value.GetString();
                    }
                    if (item.Name == "password")
                    {
                        password = item.Value.GetString();
                    }
                }
                return Tuple.Create(usernmae, password);
            }
        }
    }
}
