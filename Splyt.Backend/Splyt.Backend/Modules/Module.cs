using System;
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

        private static object OnErrorHandler(NancyContext ctx, Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
}