using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberQueuingSystem.TERMINAL.ViewModels
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

        #region Action
        public Action CloseAction { get; set; }
        #endregion

        #region Properties
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
        private BOL.Terminal_Queue.objTerminalQueue _TerminalQueue;
        public BOL.Terminal_Queue.objTerminalQueue TerminalQueue
        {
            get
            {
                return _TerminalQueue;
            }
            set
            {
                if (_TerminalQueue != value)
                {
                    _TerminalQueue = value;
                    OnPropertyChanged("TerminalQueue");
                }
            }
        }
        #endregion

        #region Constructors
        public ucButton(BOL.Terminal.objTerminal terminal, BOL.Terminal_Queue.objTerminalQueue terminal_queue)
        {
            this.Terminal = terminal;
            this.TerminalQueue = terminal_queue;
            this.Click_Command = new COMMON.RelayCommand(this.Execute_Click, this.CanClick);
        }
        #endregion

        #region Commands
        public COMMON.RelayCommand Click_Command { get; private set; }
        void Execute_Click(object para)
        {
            if (System.Windows.MessageBox.Show("Are you sure you want to transfer " + this.TerminalQueue.ToString() + " to " + this.Terminal, "TRANSFER CONFIRMATION", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
            {
                DAL.Transaction_Queue.TransactionQueue_Repository.TransferQueue(this.Terminal, this.TerminalQueue);
                DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = TerminalQueue.terminal_id, type = BOL.Task.objTask.MessageType.QueueForward });
                DAL.Task.Task_Repository.Add_Refresh_AllClientTerminal_Task();
                CloseAction();
            }
            
        }

        bool CanClick(object para)
        {
            return true;
        }
        #endregion

    }
}
