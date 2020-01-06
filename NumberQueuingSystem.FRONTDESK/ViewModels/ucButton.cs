using System;
using System.ComponentModel;
using System.Diagnostics;


namespace NumberQueuingSystem.FRONTDESK.ViewModels
{
    class ucButton : INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Methods
        public ucButton(BOL.Transaction.objTransaction transaction)
        {
            this.Transaction = transaction;
            this.Click_Command = new COMMON.RelayCommand(this.Execute_ClickCommand, this.CanClick);

            CanPress = true;
        }
        //void Delay_Press()
        //{
        //    CanPress = false;
        //    BackgroundWorker bw = new BackgroundWorker();
        //    bw.DoWork += (s, arg) =>
        //    {
        //        System.Threading.Thread.Sleep(1000);
        //    };

        //    bw.RunWorkerCompleted += (s, arg) =>
        //    {
        //        CanPress = true;
        //    };
        //    bw.RunWorkerAsync();
        //}
        #endregion

        #region Properties
        public string Trans_Name
        {
            get { return _Transaction.name; }
        }
        public string Trans_Desc
        {
            get { return Transaction.description; }
        }
        private BOL.Transaction.objTransaction _Transaction;
        public BOL.Transaction.objTransaction Transaction
        {
            get
            {
                return _Transaction;
            }
            set
            {
                if (_Transaction != value)
                {
                    _Transaction = value;
                    OnPropertyChanged("Transaction");
                }
            }
        }

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
                    OnPropertyChanged("Queue");
                }
            }
        }
        private bool _CanPress;
        public bool CanPress
        {
            get
            {
                return _CanPress;
            }
            set
            {
                if (_CanPress != value)
                {
                    _CanPress = value;
                    OnPropertyChanged("CanPress");
                }
            }
        }
        #endregion

        #region Commands
        public COMMON.RelayCommand Click_Command { get; private set; }

        void Execute_ClickCommand(object parameters)
        {
            CanPress = false;
            try
            {
                TransactionQueue = DAL.Transaction_Queue.TransactionQueue_Repository.AddNew(Transaction);
                DAL.Task.Task_Repository.Add_Refresh_AllClientTerminal_Task();
                if (Properties.Settings.Default.EnablePrinting)
                {
                    RPT.Print_Repository.Print_Queue(TransactionQueue);
                }
                Views.vPromptTicket PromptTicketV = new Views.vPromptTicket();
                ViewModels.vmPromptTicket PromptTicketVM = new vmPromptTicket(TransactionQueue);
                PromptTicketVM.CloseAction = new Action(() => { PromptTicketV.Close(); });
                PromptTicketV.DataContext = PromptTicketVM;
                PromptTicketV.ShowDialog();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Frontdesk", ex.Message, EventLogEntryType.Error);
            }
            CanPress = true;
        }

        bool CanClick(object parameters)
        {
            return true;
        }
        #endregion
    }
}
