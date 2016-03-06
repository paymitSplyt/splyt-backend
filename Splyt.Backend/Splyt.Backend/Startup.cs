using Backend;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Backend
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR(new HubConfiguration
            {
                EnableJSONP = true
            });
            app.UseNancy();
        }
    }
}