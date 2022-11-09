using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace AX.History
{
    public sealed class TransactionalHistory : ITransactionalHistory
    {
        private bool canUndo = false;
        public bool CanUndo
        {
            get { return canUndo; }
            set
            {
                if (canUndo != value)
                {
                    canUndo = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool canRedo = false;
        public bool CanRedo
        {
            get { return canRedo; }
            set
            {
                if (canRedo != value)
                {
                    canRedo = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool inTransaction = false;
        public bool InTransaction
        {
            get { return inTransaction; }
            set
            {
                if (inTransaction != value)
                {
                    inTransaction = value;
                    OnPropertyChanged();
                }
            }
        }

        private int transactionDepth = 0;
        public int TransactionDepth
        {
            get { return transactionDepth; }
            set
            {
                if (transactionDepth != value)
                {
                    transactionDepth = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isPaused = false;
        public bool IsPaused
        {
            get { return isPaused; }
            set
            {
                if (isPaused != value)
                {
                    isPaused = value;
                    OnPropertyChanged();
                }
            }
        }

        private Stack<TransactionInfo> Transactions { get; } = new Stack<TransactionInfo>();

        public event PropertyChangedEventHandler PropertyChanged;

        private Stack<HistoryEntry> Undos { get; } = new Stack<HistoryEntry>();
        private Stack<HistoryEntry> Redos { get; } = new Stack<HistoryEntry>();


        public void BeginTransaction(string name = null)
        {
            Transactions.Push(new TransactionInfo(name));
            RecalculateProperties();
        }

        public void CancelTransaction()
        {
            if (InTransaction)
            {
                Transactions.Pop();
                RecalculateProperties();
            }
        }

        public void CommitTransaction()
        {
            if (InTransaction)
            {
                var transaction = Transactions.Pop();
                Snapshot(transaction.Undo, transaction.Do, transaction.Name);
            }
        }

        public void RollbackTransaction()
        {
            if (InTransaction)
            {
                IsPaused = true;
                var transaction = Transactions.Pop();
                transaction.Undo();

                RecalculateProperties();
                IsPaused = false;
            }
        }

        public void Snapshot(Action undo, Action redo, string name = null)
        {
            Debug.Assert(undo != null);
            Debug.Assert(redo != null);

            if (undo == null)
                throw new ArgumentNullException(nameof(undo));
            if (redo == null)
                throw new ArgumentNullException(nameof(redo));

            if (Transactions.Count > 0)
            {
                Transactions.Peek().Append(new HistoryEntry(undo, redo, name));
            }
            else
            {
                if (Redos.Count > 0) Redos.Clear();
                Undos.Push(new HistoryEntry(undo, redo, name));
            }
            RecalculateProperties();

        }

        public void Undo()
        {
            if (CanUndo)
            {
                IsPaused = true;
                var entry = Undos.Pop();
                entry.undo.Invoke();
                Redos.Push(entry);

                RecalculateProperties();
                IsPaused = false;
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                IsPaused = true;
                var entry = Redos.Pop();
                entry.redo.Invoke();
                Undos.Push(entry);
                RecalculateProperties();

                IsPaused = false;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RecalculateProperties()
        {
            TransactionDepth = Transactions.Count;
            InTransaction = TransactionDepth != 0;
            CanUndo = !InTransaction && Undos.Count > 0;
            CanRedo = !InTransaction && Redos.Count > 0;
        }

        public void Clear()
        {
            if (!inTransaction)
            {
                Transactions.Clear();
                Undos.Clear();
                Redos.Clear();
                RecalculateProperties();
            }
            else
            {
                throw new InvalidOperationException("Cannot clear history while in transaction!");
            }
        }

    }
}
