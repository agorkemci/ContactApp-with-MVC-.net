namespace ContactApp.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email{ get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? Title { get; set; }
        public string? Notes { get; set; }
        public Contact(string firstName, string lastName, string? email = null, string? phone = null, string? company = null, string? title = null, string? notes = null)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Phone = phone;
            this.Company = company;
            this.Title = title;
            this.Notes = notes;
        }

        public Contact()
        {
        }
    }
}
