using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AX.History
{
    public interface ITransactionalHistory : IHistory
    {
        bool InTransaction { get; }
        int TransactionDepth { get; }

        void BeginTransaction(string name = null);
        void CommitTransaction();
        void RollbackTransaction();
        void CancelTransaction();
    }
}
