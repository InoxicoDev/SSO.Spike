using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InoxicoIdentity
{
    public class RefCodeRegistry
    {
        private readonly List<Entry> entries = new List<Entry>();

        public string CreateRefCodeForUser(string externalUserId)
        {
            var existingEntry = entries.SingleOrDefault(p => p.Name == externalUserId);
            if (existingEntry != null)
            {
                entries.Remove(existingEntry);
            }

            var newEntry = new Entry(externalUserId, Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(5));
            entries.Add(newEntry);

            return newEntry.RefCode;
        }

        class Entry
        {
            public Entry(string name, string refCode, DateTime expire)
            {
                Name = name;
                RefCode = refCode;
                Expire = expire;
            }

            public string Name { get; }
            public string RefCode { get; }
            public DateTime Expire { get; }
        }
    }
}