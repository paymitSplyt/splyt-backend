using System;

namespace Backend.DataAccess.Models
{
    public abstract class Model
    {
        protected Model()
        {
            Created = DateTime.Now;
        }

        public DateTime Created { get; set; }

        public int Id { get; set; }
    }
}