using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AX.History
{
    public struct HistoryEntry
    {
        public readonly string name;

        internal readonly Action undo;
        internal readonly Action redo;

        public HistoryEntry(Action undo, Action redo, string name)
        {
            Debug.Assert(undo != null);
            Debug.Assert(redo != null);

            this.undo = undo;
            this.redo = redo;
            this.name = name;
        }
    }
}
