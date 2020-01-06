using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;

namespace NumberQueuingSystem.FRONTDESK.ViewModels
{
    class vmFrontDesk : INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public vmFrontDesk()
        {
            Initialize();
        }

        void Initialize()
        {
            Buttons = new ObservableCollection<Views.ucButton>();
            Check_Connection();
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
                    if (!(bool)arg.Result)
                    {
                        Prompt_No_Connection();
                    }
                }
                else
                {
                    Prompt_No_Connection();
                }
                Fill_Buttons();
                Start_Query();
            };
            bw.RunWorkerAsync();
        }

        void Start_Query()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                arg.Result = DAL.Task.Task_Repository.GetFrontdeskTask();
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
                else
                {
                    Prompt_No_Connection();
                }
                    
                Start_Query();
            };
            bw.RunWorkerAsync();
        }

        void Prompt_No_Connection()
        {
            Views.vPromptConnection PromtV = new Views.vPromptConnection();
            ViewModels.vmPromptConnection PromptVM = new vmPromptConnection();
            PromptVM.CloseAction = new Action(() => { PromtV.Close(); });
            PromtV.DataContext = PromptVM;
            PromtV.ShowDialog();
        }

        void Process_Message(object obj)
        {
            if (obj != null)
            {
                if (((BOL.Task.objTask)obj).terminal_id == null)
                {
                    switch (((BOL.Task.objTask)obj).type)
                    {
                        case BOL.Task.objTask.MessageType.RefreshFrontDesk:
                            Fill_Buttons();
                            break;
                        default:
                            //nothing
                            break;
                    };
                }
            }

        }
        void Fill_Buttons()
        {
            Buttons.Clear();
            List<BOL.Transaction.objTransaction> Transactions = DAL.Transaction.Transaction_Repository.GetActiveTransaction();
            
            foreach (var obj in Transactions)
            {
                Views.ucButton butn = new Views.ucButton();
                ViewModels.ucButton btmvm = new ucButton(obj);
                butn.DataContext = btmvm;
                Buttons.Add(butn);
            }
           
        }

        #region Properties
        private ObservableCollection<Views.ucButton> _Buttons;

        public ObservableCollection<Views.ucButton> Buttons
        {
            get
            {
                return _Buttons;
            }
            set
            {
                _Buttons = value;
                OnPropertyChanged("Buttons");
            }
        }

        #endregion

    }
}
