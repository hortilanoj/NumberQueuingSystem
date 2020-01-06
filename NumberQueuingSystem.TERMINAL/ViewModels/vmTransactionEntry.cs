using System;
using System.ComponentModel;
using System.Diagnostics;

namespace NumberQueuingSystem.TERMINAL.ViewModels
{
    class vmTransactionEntry : INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Action
        public Action CloseAction { get; set; }
        #endregion

        #region Constructor
        public vmTransactionEntry(COMMON.SaveMode.Mode mode, BOL.Transaction.objTransaction transaction)
        {
            this.Mode = mode;
            this.Transaction = transaction;
            this.Cancel_Command = new COMMON.RelayCommand(this.Execute_Cancel, this.CanCancel);
            this.Save_Command = new COMMON.RelayCommand(this.Execute_Save, this.CanSave);
        }
        #endregion

        #region Properties
        private COMMON.SaveMode.Mode Mode;
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

        public string Name
        {
            get
            {
                return _Transaction.name;
            }
            set
            {
                if (_Transaction.name != value)
                {
                    _Transaction.name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Prefix
        {
            get
            {
                return _Transaction.prefix;
            }
            set
            {
                if (_Transaction.prefix != null)
                {
                    _Transaction.prefix = value;
                    OnPropertyChanged("Prefix");
                }
            }
        }
        public string Description
        {
            get
            {
                return _Transaction.description;
            }
            set
            {
                if (_Transaction.description != value)
                {
                    _Transaction.description = value;
                    OnPropertyChanged("Description");
                }
            }
        }
        public bool Active
        {
            get
            {
                return _Transaction.active;
            }
            set
            {
                if (_Transaction.active != value)
                {
                    _Transaction.active = value;
                    OnPropertyChanged("Active");
                }
            }
        }
        #endregion

        #region Methods
        #endregion

        #region Commands
        public COMMON.RelayCommand Cancel_Command { get; private set; }

        void Execute_Cancel(object para)
        {
            CloseAction();
        }

        bool CanCancel(object para)
        {
            return true;
        }

        public COMMON.RelayCommand Save_Command { get; private set; }

        void Execute_Save(object para)
        {
            try
            {
                if (this.Mode == COMMON.SaveMode.Mode.AddNew)
                {
                    if (DAL.Transaction.Transaction_Repository.Add(Transaction))
                    {
                        DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = null, type = BOL.Task.objTask.MessageType.RefreshFrontDesk });
                        CloseAction();
                    }
                }
                else
                {
                    if (DAL.Transaction.Transaction_Repository.Update(Transaction))
                    {
                        DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = null, type = BOL.Task.objTask.MessageType.RefreshFrontDesk });
                        CloseAction();
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
            }
        }
        bool CanSave(object para)
        {
            return true;
        }
        #endregion
    }
}
