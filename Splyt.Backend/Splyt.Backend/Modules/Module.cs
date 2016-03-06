using System;
using Backend.Hubs;
using Microsoft.AspNet.SignalR;
using Nancy;

namespace Backend.Modules
{
    public abstract class Module : NancyModule
    {
        protected Module(string modulePath)
            : base(modulePath)
        {
            OnError += OnErrorHandler;
        }

        protected void NotifyClients()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.All.notify();
        }

        private static object OnErrorHandler(NancyContext ctx, Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
}