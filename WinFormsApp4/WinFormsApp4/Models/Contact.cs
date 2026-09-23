using Newtonsoft.Json;

namespace PhoneBook.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public string FullName => $"{LastName} {FirstName}".Trim();

        [JsonIgnore]
        public string DisplayInfo => $"{FullName} | {Phone} | {Email}";
    }
}