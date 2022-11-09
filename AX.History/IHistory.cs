using System;
using System.ComponentModel;

namespace AX.History
{
    public interface IHistory : INotifyPropertyChanged
    {
        bool CanRedo { get; }
        bool CanUndo { get; }
        bool IsPaused { get; set; }

        void Redo();
        void Snapshot(Action undo, Action redo, string name = null);
        void Undo();
    }
}