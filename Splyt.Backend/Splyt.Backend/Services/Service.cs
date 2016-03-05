using System;
using Backend.DataAccess;

namespace Backend.Services
{
    public class Service : IDisposable
    {
        protected Service()
        {
            DataContext = new DataContext();
        }

        ~Service()
        {
            Dispose(false);
        }

        protected DataContext DataContext { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DataContext.Dispose();
            }
        }
    }
}