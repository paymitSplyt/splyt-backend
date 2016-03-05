using System;

namespace Backend.Modules
{
    public class PaymentModule : Module
    {
        public PaymentModule()
            : base("/Payment")
        {
            Post["/PayUser"] = _ => PostPayUser();
        }

        private object PostPayUser()
        {
            throw new NotImplementedException();
        }
    }
}