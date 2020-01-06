using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NumberQueuingSystem.TERMINAL.ViewModels
{
    class vmTermTrans : INotifyPropertyChanged
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

        #region Properties
        public ObservableCollection<BOL.Terminal_Transaction.objTerminalTransaction> TerminalTransactions { get; private set; }
        public ObservableCollection<BOL.Transaction.objTransaction> Transactions { get; private set; }


        private BOL.Terminal.objTerminal _Terminal;
        public BOL.Terminal.objTerminal Terminal
        {
            get
            {
                return _Terminal;
            }
            set
            {
                if (_Terminal != value)
                {
                    _Terminal = value;
                    OnPropertyChanged("Terminal");
                }
            }
        }

        private BOL.Transaction.objTransaction _Selected_Transaction;
        public BOL.Transaction.objTransaction Selected_Transaction
        {
            get
            {
                return _Selected_Transaction;
            }
            set
            {
                if (_Selected_Transaction != value)
                {
                    _Selected_Transaction = value;
                    OnPropertyChanged("Selected_Transaction");
                    OnPropertyChanged("Add_Enabled");
                }
            }
        }

        private BOL.Terminal_Transaction.objTerminalTransaction _Selected_TerminalTransaction;
        public BOL.Terminal_Transaction.objTerminalTransaction Selected_TerminalTransaction
        {
            get
            {
                return _Selected_TerminalTransaction;
            }
            set
            {
                if (_Selected_TerminalTransaction != value)
                {
                    _Selected_TerminalTransaction = value;
                    OnPropertyChanged("Selected_TerminalTransaction");
                }
            }
        }

        public bool Add_Enabled
        {
            get
            {
                return (Selected_Transaction != null ? true : false);
            }
        }

        #endregion

        #region Constructors
        public vmTermTrans(BOL.Terminal.objTerminal terminal)
        {
            TerminalTransactions = new ObservableCollection<BOL.Terminal_Transaction.objTerminalTransaction>();
            Transactions = new ObservableCollection<BOL.Transaction.objTransaction>();
            Terminal = terminal;

            this.Close_Command = new COMMON.RelayCommand(this.Execute_Close, this.CanClose);
            this.AddNew_Command = new COMMON.RelayCommand(this.Execute_AddNew, this.CanAddNew);
            this.Remove_Command = new COMMON.RelayCommand(this.Execute_Remove, this.CanRemove);
            this.MoveUp_Command = new COMMON.RelayCommand(this.Execute_MoveUp, this.CanMoveUp);
            this.MoveDown_Command = new COMMON.RelayCommand(this.Execute_MoveDown, this.CanMoveDown);

            Update_TerminalTransaction_List();
            Update_AvailableTransactions_List();
        }
        #endregion

        #region Methods
        void Update_TerminalTransaction_List()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                arg.Result = DAL.Terminal_Transaction.TerminalTransaction_Repository.GetTerminalTransactions(Terminal);
            };

            bw.RunWorkerCompleted += (s, arg) =>
            {
                if (arg.Error == null)
                {
                    if (arg.Result != null)
                    {
                        TerminalTransactions.Clear();
                        ((List<BOL.Terminal_Transaction.objTerminalTransaction>)arg.Result).ForEach(x => TerminalTransactions.Add(x));
                    }
                }
            };
            bw.RunWorkerAsync();
        }

        void Update_AvailableTransactions_List()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                arg.Result = DAL.Transaction.Transaction_Repository.GetAvailableTransactions(Terminal);
            };

            bw.RunWorkerCompleted += (s, arg) =>
            {
                if (arg.Error == null)
                {
                    if (arg.Result != null)
                    {
                        Transactions.Clear();
                        ((List<BOL.Transaction.objTransaction>)arg.Result).ForEach(x => Transactions.Add(x));
                    }
                }
            };
            bw.RunWorkerAsync();
        }
        #endregion

        #region Commands
        public COMMON.RelayCommand Close_Command { get; private set; }
        void Execute_Close(object para)
        {
            CloseAction();
        }
        bool CanClose(object obj)
        {
            return true;
        }

        public COMMON.RelayCommand AddNew_Command { get; private set; }
        void Execute_AddNew(object para)
        {
            try
            {
                DAL.Terminal_Transaction.TerminalTransaction_Repository.AddNew_Transaction(this.Terminal, Selected_Transaction);
                Update_TerminalTransaction_List();
                Update_AvailableTransactions_List();
                DAL.Task.Task_Repository.Add_Refresh_AllClientTerminal_Task();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
            }
        }
        bool CanAddNew(object para)
        {
            return true;
        }

        public COMMON.RelayCommand Remove_Command { get; private set; }
        void Execute_Remove(object para)
        {
            try
            {
                if (DAL.Terminal_Transaction.TerminalTransaction_Repository.Remove(Selected_TerminalTransaction))
                {
                    Update_TerminalTransaction_List();
                    Update_AvailableTransactions_List();
                    DAL.Task.Task_Repository.Add_Refresh_AllClientTerminal_Task();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
            }
        }
        bool CanRemove(object para)
        {
            return true;
        }

        public COMMON.RelayCommand MoveUp_Command { get; private set; }
        void Execute_MoveUp(object para)
        {
            try
            {
                if (DAL.Terminal_Transaction.TerminalTransaction_Repository.Move_Up(Selected_TerminalTransaction))
                {
                    Update_TerminalTransaction_List();
                    DAL.Task.Task_Repository.Add_Refresh_AllClientTerminal_Task();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
            }
        }
        bool CanMoveUp(object para)
        {
            return true;
        }

        public COMMON.RelayCommand MoveDown_Command { get; private set; }
        void Execute_MoveDown(object para)
        {
            try
            {
                if (DAL.Terminal_Transaction.TerminalTransaction_Repository.Move_Down(Selected_TerminalTransaction))
                {
                    Update_TerminalTransaction_List();
                    DAL.Task.Task_Repository.Add_Refresh_AllClientTerminal_Task();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
            }
        }
        bool CanMoveDown(object para)
        {
            return true;
        }
        #endregion
    }
}
