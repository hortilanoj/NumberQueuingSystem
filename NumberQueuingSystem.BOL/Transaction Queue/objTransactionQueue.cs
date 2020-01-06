using System;

namespace NumberQueuingSystem.BOL.Transaction_Queue
{
    public class objTransactionQueue
    {
        #region Objects
        public BOL.Transaction.objTransaction objTransaction { get; set; }
        #endregion

        #region Properties
        public long? id { get; set; }
        public long? terminal_id { get; set; }
        public long queue_number { get; set; }
        public DateTime date_time { get; set; }
        #endregion

        #region Constructors
        public objTransactionQueue()
        {
            id = 0;
            terminal_id = 0;
            queue_number = 0;
            objTransaction = new Transaction.objTransaction();
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            if (objTransaction == null)
                objTransaction = new Transaction.objTransaction();

            return String.Format("{0}{1:0000}", objTransaction.prefix.Trim(), queue_number);
        }
        #endregion
    }
}
