using System;
using Backend.Properties;
using Microsoft.Owin.Hosting;

namespace Backend
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var url = Settings.Default.Url;
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine($"Running on {url}");
                Console.ReadLine();
            }
        }
    }
}