using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Controls;

namespace NumberQueuingSystem.TERMINAL.ViewModels
{
    public class vmTerminal : INotifyPropertyChanged
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
        public vmTerminal()
        {
            Initialize();
        }

        #region Procedures
        void Initialize()
        {
            DAL.Base.ConnectionManager.ConnectionString = Properties.Settings.Default.ConnectionString;
            
            this.Next_Command = new COMMON.RelayCommand(this.Execute_NextCommand, this.CanNext);
            this.Recall_Command = new COMMON.RelayCommand(this.Execute_RecallCommand, this.CanRecall);
            this.Config_Command = new COMMON.RelayCommand(this.Execute_ConfigCommand, this.CanConfig);
            this.Forward_Command = new COMMON.RelayCommand(this.Execute_ForwardCommand, this.CanForward);
            this.Return_Command = new COMMON.RelayCommand(this.Execute_ReturnCommand, this.CanReturn);

            CanPress = true;
            Check_Connection();
        }

        void Update_Terminal()
        {

            try
            {
                this.Terminal = DAL.Terminal.Terminal_Repository.GetTerminal(Properties.Settings.Default.TerminalId);
                this.Queue = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this._Terminal);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
            }
        }

        void Check_Connection()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                arg.Result = DAL.Base.ConnectionManager.Check_Connection();

            };
            bw.RunWorkerCompleted += (s, arg) =>
            {
               if (arg.Error == null)
               {
                   if ((bool)arg.Result)
                   {
                       Update_Terminal();
                       Update_WaitingList();
                       Update_HistoryList();
                       Start_Query_Task();
                   }
                   else
                   {
                       Check_Connection();
                   }
               }
               else
               {
                   Check_Connection();
               }
            };
            bw.RunWorkerAsync();
        }

        void Start_Query_Task()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                arg.Result = DAL.Task.Task_Repository.GetTerminalClientTask(Terminal);
            };

            bw.RunWorkerCompleted += (s, arg) =>
            {
                if (arg.Error == null)
                {
                    if (arg.Result != null)
                    {
                        Process_Message(arg.Result);
                    }
                }
                Start_Query_Task();
            };
            bw.RunWorkerAsync();
        }

        void Process_Message(object obj)
        {
            if (obj != null)
            {
                switch (((BOL.Task.objTask)obj).type)
                {
                    case BOL.Task.objTask.MessageType.RefreshClientTerminal:
                        Update_WaitingList();
                        Update_HistoryList();
                        break;
                    default:
                        //nothing
                        break;
                };
            }

        }

        void Update_WaitingList()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                try
                {
                    arg.Result = DAL.Transaction_Queue.TransactionQueue_Repository.GetWaitingList(this.Terminal);
                }
                catch(Exception ex)
                {
                    EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
                }
                
            };
            bw.RunWorkerCompleted += (s, arg) =>
            {
                if (arg.Error == null)
                {
                    if (arg.Result != null)
                    {
                        Waiting_List.Clear();
                        ((List<BOL.Transaction_Queue.objTransactionQueue>)arg.Result).ForEach(x => Waiting_List.Add(x));
                    }
                }
            };
            bw.RunWorkerAsync();
        }

        void Update_HistoryList()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                try
                {
                    arg.Result = DAL.Terminal_Queue.TerminalQueue_Repository.GetTerminalQueueHistory(this.Terminal);
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
                }

            };
            bw.RunWorkerCompleted += (s, arg) =>
            {
                if (arg.Error == null)
                {
                    if (arg.Result != null)
                    {
                        History_List.Clear();
                        ((List<BOL.Terminal_Queue.objTerminalQueue>)arg.Result).ForEach(x => History_List.Add(x));
                    }
                }
            };
            bw.RunWorkerAsync();
        }

        void Delay_Press()
        {
            CanPress = false;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                System.Threading.Thread.Sleep(1500);
            };

            bw.RunWorkerCompleted += (s, arg) =>
            {
                CanPress = true;
            };
            bw.RunWorkerAsync();
        }

        public void Show_WaitingList_Options()
        {
            if (_Selected_Waiting != null)
            {
                ContextMenu Menus = new ContextMenu();
                MenuItem menuSetAsCurrent = new MenuItem()
                {
                    Header = "Set as Current"
                };
                menuSetAsCurrent.Click += (s, a) =>
                {
                    if (System.Windows.MessageBox.Show("Are you sure you want to set " + Selected_Waiting + " to current?", "SET CONFIRMATION", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
                    {
                        try
                        {
                            if (DAL.Terminal_Queue.TerminalQueue_Repository.Set_Waiting_As_Current(Selected_Waiting, Terminal))
                            {
                                Queue = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this._Terminal);
                                DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = this._Terminal.id, type = BOL.Task.objTask.MessageType.CurrentIsSet });
                                DAL.Task.Task_Repository.Add_Refresh_AllClientTerminal_Task();
                            }
                        }
                        catch (Exception ex)
                        {
                            EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
                        }
                    }
                };
                Menus.Items.Add(menuSetAsCurrent);
                Menus.IsOpen = true;
            }

        }

        public void Show_History_Options()
        {
            if (_Selected_History != null)
            {
                ContextMenu Menus = new ContextMenu();
                MenuItem menuSetAsCurrent = new MenuItem()
                {
                    Header = "Set as Current"
                };
                menuSetAsCurrent.Click += (s, a) =>
                {
                    if (System.Windows.MessageBox.Show("Are you sure you want to set " + Selected_History + " to current?", "SET CONFIRMATION", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
                    {
                        try
                        {
                            if (DAL.Terminal_Queue.TerminalQueue_Repository.Set_History_As_Current(Selected_History))
                            {
                                Queue = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this._Terminal);
                                DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = this._Terminal.id, type = BOL.Task.objTask.MessageType.CurrentIsSet });
                                DAL.Task.Task_Repository.Add_Refresh_AllClientTerminal_Task();
                            }
                        }
                        catch (Exception ex)
                        {
                            EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
                        }
                    }
                };
                Menus.Items.Add(menuSetAsCurrent);
                Menus.IsOpen = true;
            }

        }

        #endregion

        #region Properties

        private ObservableCollection<BOL.Transaction_Queue.objTransactionQueue> _Waiting_List;
        public ObservableCollection<BOL.Transaction_Queue.objTransactionQueue> Waiting_List
        {
            get
            {
                if (_Waiting_List == null)
                    _Waiting_List = new ObservableCollection<BOL.Transaction_Queue.objTransactionQueue>();
                return _Waiting_List;
            }
            set
            {
                if (_Waiting_List != value)
                {
                    _Waiting_List = value;
                    OnPropertyChanged("Waiting_List");
                }
            }
        }

        private BOL.Transaction_Queue.objTransactionQueue _Selected_Waiting;
        public BOL.Transaction_Queue.objTransactionQueue Selected_Waiting
        {
            get
            {
                return _Selected_Waiting;
            }
            set
            {
                if (_Selected_Waiting != value)
                {
                    _Selected_Waiting = value;
                    OnPropertyChanged("Selected_Waiting");
                }
            }
        }

        private ObservableCollection<BOL.Terminal_Queue.objTerminalQueue> _History_List;
        public ObservableCollection<BOL.Terminal_Queue.objTerminalQueue> History_List
        {
            get
            {
                if (_History_List == null)
                    _History_List = new ObservableCollection<BOL.Terminal_Queue.objTerminalQueue>();
                return _History_List;
            }
            set
            {
                if (_History_List != value)
                {
                    _History_List = value;
                    OnPropertyChanged("HistoryList");
                }
            }
        }
        private BOL.Terminal_Queue.objTerminalQueue _Selected_History;
        public BOL.Terminal_Queue.objTerminalQueue Selected_History
        {
            get
            {
                return _Selected_History;
            }
            set
            {
                if (_Selected_History != value)
                {
                    _Selected_History = value;
                    OnPropertyChanged("Selected_History");
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

        private BOL.Terminal_Queue.objTerminalQueue _Queue;
        public BOL.Terminal_Queue.objTerminalQueue Queue
        {
            get
            {
                return _Queue;
            }
            set
            {
                if (_Queue != value)
                {
                    _Queue = value;
                    OnPropertyChanged("Queue");
                }
                
            }
        }

        #endregion

        #region Commands

        public COMMON.RelayCommand Forward_Command { get; private set; }

        void Execute_ForwardCommand(object para)
        {
            Delay_Press();
            if (!this.Queue.is_done)
            {
                try
                {
                    Views.vTerminalForwarding TerminalForwardingv = new Views.vTerminalForwarding();
                    ViewModels.vmTerminalForwarding TerminalForwardingvm = new vmTerminalForwarding(this._Terminal, this.Queue);
                    TerminalForwardingv.DataContext = TerminalForwardingvm;
                    TerminalForwardingvm.CloseAction = new Action(() =>
                    {
                        TerminalForwardingv.Close();
                        Queue = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this._Terminal);
                    });
                    TerminalForwardingv.Loaded += (s, a) =>
                        {
                            TerminalForwardingv.Left = System.Windows.SystemParameters.WorkArea.Width - TerminalForwardingv.Width;
                            TerminalForwardingv.Top = System.Windows.SystemParameters.WorkArea.Height - TerminalForwardingv.Height;
                        };
                    TerminalForwardingv.ShowDialog();
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
                }
            }
           
        }

        bool CanForward(object para)
        {
            return true;
        }

        public COMMON.RelayCommand Next_Command { get; private set; }

        void Execute_NextCommand(object parameters)
        {
            Delay_Press();
            try
            {
                DAL.Terminal_Queue.TerminalQueue_Repository.Next(this._Terminal, this.Queue);
                Queue = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this._Terminal);
                DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = this._Terminal.id, type = BOL.Task.objTask.MessageType.NextQueue });
                DAL.Task.Task_Repository.Add_Refresh_AllClientTerminal_Task();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
            }
        }

        bool CanNext(object parameter)
        {
            return true;
        }

        public COMMON.RelayCommand Recall_Command { get; private set; }

        void Execute_RecallCommand(object parameters)
        {
            Delay_Press();
            if (!this.Queue.is_done)
            {
                try
                {
                    this.Queue = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this._Terminal);
                    DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = this._Terminal.id, type = BOL.Task.objTask.MessageType.Recall });
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
                }
                
            }
        }

        bool CanRecall(object parameters)
        {
            return true;
        }

        public COMMON.RelayCommand Config_Command { get; private set; }

        void Execute_ConfigCommand(object parameters)
        {
            Delay_Press();
            Views.vConfig Configv = new Views.vConfig();
            ViewModels.vmConfig Configvm = new vmConfig();
            Configvm.CloseAction = new Action(() =>
            {
                Configv.Close();
                //CloseAction();
            });
            Configv.DataContext = Configvm;
            Configv.ShowDialog();
        }
        bool CanConfig(object parameters)
        {
            return true;
        }

        public COMMON.RelayCommand Return_Command { get; private set; }

        void Execute_ReturnCommand(object parameters)
        {
            Delay_Press();
            if (!this.Queue.is_done)
            {
                try
                {
                    if (DAL.Transaction_Queue.TransactionQueue_Repository.ReturnQueue(Queue))
                    {
                        Queue = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this._Terminal);
                        DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = this._Terminal.id, type = BOL.Task.objTask.MessageType.NextQueue });
                        DAL.Task.Task_Repository.Add_Refresh_AllClientTerminal_Task();
                    }
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
                }
            }
        }
        bool CanReturn(object parameters)
        {
            return true;
        }

        
        #endregion



    }
}
