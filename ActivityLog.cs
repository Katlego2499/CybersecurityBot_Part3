using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityBot_Part3
{
    public class ActivityLog
    {
        private List<string> _log = new List<string>();

        public void AddEntry(string action)
        {
            string entry = $"[{DateTime.Now:HH:mm:ss}] {action}";
            _log.Add(entry);
        }

        public List<string> GetRecentEntries(int count = 10)
        {
            return _log.Skip(Math.Max(0, _log.Count - count)).ToList();
        }

        public List<string> GetAllEntries()
        {
            return new List<string>(_log);
        }
    }
}
