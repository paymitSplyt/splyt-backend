using System.Collections.Generic;

namespace Backend.DataAccess.Models
{
    public class User : Model
    {
        public virtual ICollection<CartItem_User> CartItem_Users { get; set; }

        public long Phonenumber { get; set; }
    }
}