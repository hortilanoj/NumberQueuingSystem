using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace NumberQueuingSystem.FRONTDESK.ViewModels
{
    class vmPromptTicket : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Actions
        public Action CloseAction { get; set; }
        #endregion

        #region Constructors
        public vmPromptTicket(BOL.Transaction_Queue.objTransactionQueue transacton_queue)
        {
            TransactionQueue = transacton_queue;
            Delay_Prompt();
        }
        #endregion

        #region Properties
        private BOL.Transaction_Queue.objTransactionQueue _TransactionQueue;
        public BOL.Transaction_Queue.objTransactionQueue TransactionQueue
        {
            get
            {
                return _TransactionQueue;
            }
            set
            {
                if (_TransactionQueue != value)
                {
                    _TransactionQueue = value;
                    OnPropertyChanged("TransactionQueue");
                }
            }
        }
        #endregion

        #region Methods
        void Delay_Prompt()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                System.Threading.Thread.Sleep(2000);
            };

            bw.RunWorkerCompleted += (s, arg) =>
            {
                CloseAction();
            };
            bw.RunWorkerAsync();
        }
        #endregion
    }
}
