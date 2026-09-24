using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlchemistTable.Core.Entities
{
    public class Alchemist : EntityBase
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //TODO: Consider using a more secure way to store passwords, like hashing
        public List<Ingredient> Inventory { get; set; }
        //TODO: Change inventory so it can store multiple different things, like potions, ingredients, etc.

        private Alchemist() { }
        public Alchemist(string name, string email, string password, List<Ingredient>? inventory)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Password = password;
            Inventory = inventory ?? new List<Ingredient>();
        }
    }

}
