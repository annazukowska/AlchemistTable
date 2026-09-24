using AlchemistTable.Application.DTOs;
using AlchemistTable.Core.Entities;

namespace AlchemistTable.Infrastructure.Services.InMemory
{
    public class AlchemistRegistry
    {
        private readonly List<Alchemist> _alchemists  = new();

        public IEnumerable<Alchemist> GetAll() => _alchemists;

        public AlchemistRegistry()
        {
        }

        public AlchemistRegistry(List<Alchemist> alchemists)
        {
            _alchemists = alchemists;
        }

        public Alchemist? Get(Guid id) => _alchemists.FirstOrDefault(a => a.Id == id);

        public void Add(Alchemist alchemist) => _alchemists.Add(alchemist);

        public bool Remove(Guid id)
        {
            var alchemist = Get(id);
            if (alchemist == null) return false;
            _alchemists.Remove(alchemist);
            return true;
        }

        public Alchemist? Authenticate(AlchemistLoginRequest login) 
        {
            var alchemist = _alchemists.FirstOrDefault(x => x.Email == login.Email && x.Password == login.Password);
            return alchemist;
        }
    }

}
