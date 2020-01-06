using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;

namespace NumberQueuingSystem.TERMINAL.ViewModels
{
    class vmConfig : INotifyPropertyChanged
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

        #region Constructors
        public vmConfig()
        {
            Terminals = new ObservableCollection<BOL.Terminal.objTerminal>();
            Update_Terminal_Collections();
            Transactions = new ObservableCollection<BOL.Transaction.objTransaction>();
            Update_Transaction_Collections();
            Update_Display_Setting();

            this.Terminal_AddNew = new COMMON.RelayCommand(this.Execute_AddNew_Terminal, this.CanAddNew);
            this.Terminal_Update = new COMMON.RelayCommand(this.Execute_Update_Terminal, this.CanTerUpdate);
            this.Terminal_Delete = new COMMON.RelayCommand(this.Execute_Delete_Terminal, this.CanTerDelete);
            this.Terminal_MoveUp = new COMMON.RelayCommand(this.Execute_TermnalMoveUp, this.CanMoveUp);
            this.Terminal_MoveDown = new COMMON.RelayCommand(this.Execute_TerminalMoveDown, this.CanMoveDown);

            this.Transaction_AddNew = new COMMON.RelayCommand(this.Execute_AddNew_Transaction, this.CanTransAdd);
            this.Transaction_List = new COMMON.RelayCommand(this.Execute_Trans_List, this.CanTransList);
            this.Transaction_Update = new COMMON.RelayCommand(this.Execute_Update_Transaction, this.CanTransUpdate);
            this.Transaction_Delete = new COMMON.RelayCommand(this.Execute_Delete_Transaction, this.CanTransDelete);

            this.Setup_Command = new COMMON.RelayCommand(this.Execute_Setup, this.CanSetup);
            this.ApplyDisplay_Command = new COMMON.RelayCommand(this.Execute_ApplyDisplay, this.CanApplyDisplay);
            this.ResetTransactionQueue_Command = new COMMON.RelayCommand(this.Execute_RestTransactionQueue, this.CanReset);
            this.PlayNext_Command = new COMMON.RelayCommand(this.Execute_PlayNextCommand, this.CanPlayNext);
        }
        #endregion

        #region Properties
        public ObservableCollection<BOL.Terminal.objTerminal> Terminals { get; private set; }

        private BOL.Terminal.objTerminal _Selected_Terminal;
        public BOL.Terminal.objTerminal Selected_Terminal
        {
            get
            {
                return _Selected_Terminal;
            }
            set
            {
                if (_Selected_Terminal != value)
                {
                    _Selected_Terminal = value;
                    OnPropertyChanged("Selected_Terminal");
                }
            }
        }

        public ObservableCollection<BOL.Transaction.objTransaction> Transactions { get; private set; }

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
                }
            }
        }


        private BOL.Display_Settings.objDisplaySettings _Display_Setting;
        public BOL.Display_Settings.objDisplaySettings Display_Setting
        {
            get
            {
                return _Display_Setting;
            }
            set
            {
                if (_Display_Setting != value)
                {
                    _Display_Setting = value;
                    OnPropertyChanged("Display_Setting");
                    OnPropertyChanged("Header_Background_Color");
                    OnPropertyChanged("Header_Title_Color");
                    OnPropertyChanged("Header_Date_Color");
                    OnPropertyChanged("Header_Time_Color");
                }
            }
        }

        private Color _Header_Background_Color;
        public Color Header_Background_Color
        {
            get
            {
                return _Header_Background_Color;
            }
            set
            {
                if (_Header_Background_Color != value)
                {
                    _Header_Background_Color = value;
                    OnPropertyChanged("Header_Background_Color");
                    _Display_Setting.header_background_color = value.ToString();
                    OnPropertyChanged("Display_Setting");
                }
            }
        }

        private Color _Header_Title_Color;
        public Color Header_Title_Color
        {
            get
            {
                return _Header_Title_Color;
            }
            set
            {
                if (_Header_Title_Color != value)
                {
                    _Header_Title_Color = value;
                    OnPropertyChanged("Header_Title_Color");
                    _Display_Setting.header_title_color = value.ToString();
                    OnPropertyChanged("Display_Setting");
                }
            }
        }

        private Color _Header_Date_Color;
        public Color Header_Date_Color
        {
            get
            {
                return _Header_Date_Color;
            }
            set
            {
                if (_Header_Date_Color != value)
                {
                    _Header_Date_Color = value;
                    OnPropertyChanged("Header_Date_Color");
                    _Display_Setting.header_date_color = value.ToString();
                    OnPropertyChanged("Display_Setting");
                }
            }
        }

        private Color _Header_Time_Color;
        public Color Header_Time_Color
        {
            get
            {
                return _Header_Time_Color;
            }
            set
            {
                if (_Header_Time_Color != value)
                {
                    _Header_Time_Color = value;
                    OnPropertyChanged("Header_Time_Color");
                    _Display_Setting.header_time_color = value.ToString();
                    OnPropertyChanged("Display_Setting");
                }
            }
        }
        #endregion

        #region Methods
        void Update_Display_Setting()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                arg.Result = DAL.Dislay_Settings.DisplaySettings_Repository.GetCurrentDisplaySettings();
            };

            bw.RunWorkerCompleted += (s, arg) =>
            {
                if (arg.Error == null)
                {
                    if (arg.Result != null)
                    {
                        Display_Setting = (BOL.Display_Settings.objDisplaySettings)arg.Result;
                        Convert_String_To_Color();
                    }
                }
            };
            bw.RunWorkerAsync();
        }

        void Update_Terminal_Collections()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                arg.Result = DAL.Terminal.Terminal_Repository.GetSortedAllTerminals();
            };

            bw.RunWorkerCompleted += (s, arg) =>
            {
                if (arg.Error == null)
                {
                    if (arg.Result != null)
                    {
                        Terminals.Clear();
                        ((List<BOL.Terminal.objTerminal>)arg.Result).ForEach(x => Terminals.Add(x));
                    }
                }
            };
            bw.RunWorkerAsync();
        }

        void Update_Transaction_Collections()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                arg.Result = DAL.Transaction.Transaction_Repository.GetAll();
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

        void Convert_String_To_Color()
        {
            Header_Background_Color = (Color)ColorConverter.ConvertFromString(_Display_Setting.header_background_color);
            Header_Title_Color = (Color)ColorConverter.ConvertFromString(_Display_Setting.header_title_color);
            Header_Date_Color = (Color)ColorConverter.ConvertFromString(_Display_Setting.header_date_color);
            Header_Time_Color = (Color)ColorConverter.ConvertFromString(_Display_Setting.header_time_color);
        }
        #endregion 

        #region Commands
        public COMMON.RelayCommand Terminal_AddNew { get; private set; }
        void Execute_AddNew_Terminal(object para)
        {
            Views.vTerminalEntry TermEntryv = new Views.vTerminalEntry();
            ViewModels.vmTerminalEntry TermEntryvm = new vmTerminalEntry(COMMON.SaveMode.Mode.AddNew, new BOL.Terminal.objTerminal());
            TermEntryv.DataContext = TermEntryvm;
            if (TermEntryvm.CloseAction == null)
            TermEntryvm.CloseAction = new Action(() => 
                {
                    TermEntryv.Close();
                    Update_Terminal_Collections();
                });
            TermEntryv.ShowDialog();
        }
        bool CanAddNew(object para)
        {
            return true;
        }

        public COMMON.RelayCommand Terminal_Update { get; private set; }
        void Execute_Update_Terminal(object para)
        {
            Views.vTerminalEntry TransactionEntryv = new Views.vTerminalEntry();
            ViewModels.vmTerminalEntry TransactionEntryvm = new vmTerminalEntry(COMMON.SaveMode.Mode.Update, Selected_Terminal);
            TransactionEntryv.DataContext = TransactionEntryvm;
            TransactionEntryvm.CloseAction = new Action(() =>
            {
                TransactionEntryv.Close();
                Update_Terminal_Collections();
            });
            TransactionEntryv.ShowDialog();
        }
        bool CanTerUpdate(object para)
        {
            return true;
        }

        public COMMON.RelayCommand Terminal_Delete { get; private set; }
        void Execute_Delete_Terminal(object para)
        {
            if (System.Windows.MessageBox.Show("Are you sure you want to delete " + Selected_Terminal.name, 
                "DELETE CONFIRMATION", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    if (DAL.Terminal.Terminal_Repository.Delete(Selected_Terminal))
                    {
                        Update_Terminal_Collections();
                    }
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
                }
               
            }
        }
        bool CanTerDelete(object para)
        {
            return true;
        }

        public COMMON.RelayCommand Terminal_MoveUp { get; private set; }
        void Execute_TermnalMoveUp(object args)
        {
            try
            {
                if (DAL.Terminal.Terminal_Repository.Move_Up(Selected_Terminal))
                {
                    DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = null, type = BOL.Task.objTask.MessageType.LoadTerminals });
                    Update_Terminal_Collections();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
            }
        }
        bool CanMoveUp(object args)
        {
            return true;
        }

        public COMMON.RelayCommand Terminal_MoveDown{ get; private set; }
        void Execute_TerminalMoveDown(object args)
        {
            try
            {
                if (DAL.Terminal.Terminal_Repository.Move_Down(Selected_Terminal))
                {
                    DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = null, type = BOL.Task.objTask.MessageType.LoadTerminals });
                    Update_Terminal_Collections();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
            }
        }
        bool CanMoveDown(object args)
        {
            return true;
        }

        public COMMON.RelayCommand Transaction_AddNew { get; private set; }
        void Execute_AddNew_Transaction(object para)
        {
            Views.vTransactionEntry TransEntryv = new Views.vTransactionEntry();
            ViewModels.vmTransactionEntry TransEntryvm = new vmTransactionEntry(COMMON.SaveMode.Mode.AddNew, new BOL.Transaction.objTransaction());
            TransEntryv.DataContext = TransEntryvm;
            if (TransEntryvm.CloseAction == null)
            TransEntryvm.CloseAction = new Action(() =>
            {
                Update_Transaction_Collections();
                TransEntryv.Close();
            });
            TransEntryv.ShowDialog();
        }
        bool CanTransAdd(object para)
        {
            return true;
        }

        public COMMON.RelayCommand Transaction_List { get; private set; }
        void Execute_Trans_List(object para)
        {
            Views.vTermTrans TermTransv = new Views.vTermTrans();
            ViewModels.vmTermTrans TermTransvm = new ViewModels.vmTermTrans(Selected_Terminal);
            TermTransv.DataContext = TermTransvm;
            TermTransvm.CloseAction = new Action(() =>
            {
                Update_Transaction_Collections();
                TermTransv.Close();
            });
            TermTransv.ShowDialog();
        }
        bool CanTransList(object para)
        {
            return true;
        }

        public COMMON.RelayCommand Transaction_Update { get; private set; }
        void Execute_Update_Transaction(object para)
        {
            Views.vTransactionEntry TransEntryv = new Views.vTransactionEntry();
            ViewModels.vmTransactionEntry TransEntryvm = new vmTransactionEntry(COMMON.SaveMode.Mode.Update, Selected_Transaction);
            TransEntryv.DataContext = TransEntryvm;
            if (TransEntryvm.CloseAction == null)
                TransEntryvm.CloseAction = new Action(() =>
                {
                    Update_Transaction_Collections();
                    TransEntryv.Close();
                });
            TransEntryv.ShowDialog();
        }
        bool CanTransUpdate(object para)
        {
            return true;
        }

        public COMMON.RelayCommand Transaction_Delete{ get; private set; }
        void Execute_Delete_Transaction(object para)
        {
            if (System.Windows.MessageBox.Show("Are you sure you want to delete " + Selected_Transaction.name,
                "DELETE CONFIRMATION", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    if (DAL.Transaction.Transaction_Repository.Delete(Selected_Transaction))
                    {
                        Update_Transaction_Collections();
                        DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = null, type = BOL.Task.objTask.MessageType.RefreshFrontDesk });
                    }
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
                }
                
            }
        }
        bool CanTransDelete(object para)
        {
            return true;
        }

        public COMMON.RelayCommand Setup_Command { get; private set; }
        void Execute_Setup(object para)
        {
            Views.vConnection ConnectionV = new Views.vConnection();
            ViewModels.vmConnection ConnectionVM = new ViewModels.vmConnection();
            ConnectionV.DataContext = ConnectionVM;
            ConnectionVM.CloseAction = new Action(() =>
            {
                ConnectionV.Close();
            });
            ConnectionV.ShowDialog();

            //CloseAction();
        }
        bool CanSetup(object para)
        {
            return true;
        }

        public COMMON.RelayCommand ApplyDisplay_Command { get; private set; }
        void Execute_ApplyDisplay(object para)
        {
            try
            {
                if (DAL.Dislay_Settings.DisplaySettings_Repository.AddNew_DisplaySetting(Display_Setting))
                {
                    DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = null, type = BOL.Task.objTask.MessageType.LoadTerminals });
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
            }
        }
        bool CanApplyDisplay(object para)
        {
            return true;
        }

        public COMMON.RelayCommand ResetTransactionQueue_Command { get; private set; }
        void Execute_RestTransactionQueue(object args)
        {
            if (System.Windows.MessageBox.Show("Are you sure you want to reset queue numbering?", "RESET CONFIRMATION", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    List<BOL.Terminal.objTerminal> List = DAL.Terminal.Terminal_Repository.GetActiveTerminals();
                    DAL.Transaction_Queue.TransactionQueue_Repository.DeleteAllTransactionQueue();
                    List.ForEach(x => DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = x.id, type = BOL.Task.objTask.MessageType.RefreshTerminal }));
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
                }
            }
        }
        bool CanReset(object args)
        {
            return true;
        }

        public COMMON.RelayCommand PlayNext_Command { get; private set; }

        void Execute_PlayNextCommand(object parameters)
        {
            try
            {
                DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = null, type = BOL.Task.objTask.MessageType.ChangeMedia });
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
            }
        }
        bool CanPlayNext(object parameters)
        {
            return true;
        }
        #endregion


    }
}
