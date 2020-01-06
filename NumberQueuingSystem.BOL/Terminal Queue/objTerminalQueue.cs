using System;

namespace NumberQueuingSystem.BOL.Terminal_Queue
{
    public class objTerminalQueue
    {
        #region Properties
        public long? id { get; set; }
        public long? transaction_id 
        {
            get
            {
                if (objTransactionQueue == null)
                    objTransactionQueue = new Transaction_Queue.objTransactionQueue();

                if (objTransactionQueue.objTransaction == null)
                    objTransactionQueue.objTransaction = new Transaction.objTransaction();
                return objTransactionQueue.objTransaction.id;
            }
            set
            {
                if (objTransactionQueue == null)
                    objTransactionQueue = new Transaction_Queue.objTransactionQueue();

                if (objTransactionQueue.objTransaction == null)
                    objTransactionQueue.objTransaction = new Transaction.objTransaction();
                objTransactionQueue.objTransaction.id = value;
            }
        }
        public long? terminal_id { get; set; }
        public long? transaction_queue_id 
        { 
            get
            {
                if (objTransactionQueue == null)
                    objTransactionQueue = new Transaction_Queue.objTransactionQueue();
                return objTransactionQueue.id;
            }
            set
            {
                if (objTransactionQueue == null)
                    objTransactionQueue = new Transaction_Queue.objTransactionQueue();
                objTransactionQueue.id = value;
            }
        }
        public bool is_done { get; set; }
        #endregion

        #region Objects
        public BOL.Transaction_Queue.objTransactionQueue objTransactionQueue { get; set; }
        #endregion

        #region Constructors
        public objTerminalQueue()
        {
            id = 0;
            objTransactionQueue = new Transaction_Queue.objTransactionQueue();
            terminal_id = 0;
            is_done = true;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            if (objTransactionQueue == null)
                objTransactionQueue = new Transaction_Queue.objTransactionQueue();

            if (objTransactionQueue.objTransaction == null)
                objTransactionQueue.objTransaction = new Transaction.objTransaction();

            if (id > 0)
                return String.Format("{0}{1:0000}", objTransactionQueue.objTransaction.prefix.Trim(), objTransactionQueue.queue_number);
            else
                return "";
        }
        #endregion

    }
}
