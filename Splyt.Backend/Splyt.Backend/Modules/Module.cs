using Nancy;

namespace Backend.Modules
{
    public abstract class Module : NancyModule
    {
        protected Module(string modulePath)
            : base(modulePath)
        {
        }
    }
}