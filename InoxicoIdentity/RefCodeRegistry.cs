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
            var existingEntry = entries.SingleOrDefault(p => p.ExternalUserId == externalUserId);
            if (existingEntry != null)
            {
                entries.Remove(existingEntry);
            }

            var newEntry = new Entry(externalUserId, Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(5));
            entries.Add(newEntry);

            return newEntry.RefCode;
        }

        public string GetExternalUserId(string refCode)
        {
            var entry = entries.Single(p => p.RefCode == refCode);
            if (entry == null)
            {
                return null;
            }

            entries.Remove(entry);

            if (entry.Expire <= DateTime.Now)
            {
                return null;
            }

            return entry.ExternalUserId;
        }

        class Entry
        {
            public Entry(string name, string refCode, DateTime expire)
            {
                ExternalUserId = name;
                RefCode = refCode;
                Expire = expire;
            }

            public string ExternalUserId { get; }
            public string RefCode { get; }
            public DateTime Expire { get; }
        }
    }
}