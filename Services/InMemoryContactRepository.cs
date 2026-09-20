using ContactApp.Models;

namespace ContactApp.Services
{
    public class InMemoryContactRepository : IContactRepository
    {

        private readonly List<Contact> _contacts;
        private int _nextId;
        public InMemoryContactRepository()
        {
            this._contacts = new List<Contact>();
            var seedContacts = new List<Contact>
            {
                new Contact("John", "Doe", "john.doe@example.com", "123-456-7890", "ABC Inc.", "Manager", "Initial contact"),
                new Contact("Emily", "Clarke", "emily.clarke@example.com", "212-555-0148", "Nova Tech", "Software Engineer", "Referred by a colleague"),
                new Contact("Mehmet", "Yılmaz", "mehmet.yilmaz@example.com", "532-111-2233", "Yılmaz Holding", "Finance Director", "Met at industry conference"),
                new Contact("Sara", "Ahmed", "sara.ahmed@example.com", "070-9988-7766", "Global Logistics", "Operations Lead", "Follow up next quarter"),
                new Contact("Carlos", "Mendes", "carlos.mendes@example.com", "011-4455-6677", "Sunrise Marketing", "Creative Director", "Interested in partnership"),
            };
            foreach (var contact in seedContacts)
            {
                contact.Id = _nextId++;
                this._contacts.Add(contact);
            }
            
        }
        public Contact Add(Contact contact)
        {
            contact.Id = _nextId++;
            this._contacts.Add(contact);
            return contact;
        }

        public bool Delete(int id)
        {
            var existingContact = GetById(id);
            if(existingContact == null)
            {
                return false;
            }
            else
            {
                _contacts.Remove(existingContact);
                return true;
            }

        }

        public IEnumerable<Contact> GetAll()
        {
            return _contacts
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName);
        }

        public Contact? GetById(int id)
        {
            return _contacts.FirstOrDefault(c => c.Id == id);
        }

        public bool Update(Contact contact)
        {
            var existingContact = GetById(contact.Id);
            if (existingContact == null)
            {
                return false;
            }

            existingContact.FirstName = contact.FirstName;
            existingContact.LastName = contact.LastName;
            existingContact.Email = contact.Email;
            existingContact.Phone = contact.Phone;
            existingContact.Company = contact.Company;
            existingContact.Title = contact.Title;
            existingContact.Notes = contact.Notes;
            return true;    
        }
    }
}
