using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AX.History
{
    public class TransactionInfo
    {
        private List<HistoryEntry> entries = new List<HistoryEntry>();

        public string Name { get; private set; }

        public TransactionInfo(string name)
        {
            Name = name;
        }

        public void Append(HistoryEntry historyEntry)
        {
            entries.Add(historyEntry);
        }

        public void Do()
        {
            foreach (var entry in entries)
            {
                entry.redo();
            }
        }

        public void Undo()
        {
            foreach (var entry in entries.Reverse<HistoryEntry>())
            {
                entry.undo();
            }
        }
    }
}
