using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhoneBook.Models;
using Newtonsoft.Json;

namespace PhoneBook.Services
{
    public class DataService
    {
        private readonly string _filePath = "contacts.json";
        private List<Contact> _contacts = new();
        private int _nextId = 1;

        public List<Contact> Contacts => _contacts.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();

        public DataService() => Load();

        public void Add(Contact contact)
        {
            contact.Id = _nextId++;
            _contacts.Add(contact);
            Save();
        }

        public void Update(Contact contact)
        {
            var existing = _contacts.FirstOrDefault(c => c.Id == contact.Id);
            if (existing != null)
            {
                existing.FirstName = contact.FirstName;
                existing.LastName = contact.LastName;
                existing.Phone = contact.Phone;
                existing.Email = contact.Email;
                Save();
            }
        }

        public void Delete(int id)
        {
            var contact = _contacts.FirstOrDefault(c => c.Id == id);
            if (contact != null)
            {
                _contacts.Remove(contact);
                Save();
            }
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(_contacts, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public void Load()
        {
            if (!File.Exists(_filePath)) return;
            try
            {
                var json = File.ReadAllText(_filePath);
                _contacts = JsonConvert.DeserializeObject<List<Contact>>(json) ?? new List<Contact>();
                _nextId = _contacts.Any() ? _contacts.Max(c => c.Id) + 1 : 1;
            }
            catch
            {
                _contacts = new List<Contact>();
                _nextId = 1;
            }
        }
    }
}