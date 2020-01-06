using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;
using System.Media;
using System.Diagnostics;
using System.Windows;

namespace NumberQueuingSystem.DISPLAY.ViewModels
{
    class vmTerminal : INotifyPropertyChanged
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
        //public Action CloseAction { get; set; }
        #endregion

        #region Constructors
        public vmTerminal(BOL.Terminal.objTerminal terminal, COMMON.DataEvent main_event)
        {
            this.Voice = new System.Speech.Synthesis.SpeechSynthesizer()
            {
                Volume = 100
            };
            this.Timer = new System.Windows.Threading.DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            Timer.Tick += (s, a) => { Animate_Notification(); };
            this.NotificationSound = new SoundPlayer(Properties.Resources.doorbell_sound);

            Terminal = terminal;
            MainEvent = main_event;
            MainEvent.OnDataConfirm += MainEvent_OnDataConfirm;
            IsVisible = Visibility.Visible;

            Initialize();

        }
        #endregion

        #region Properties

        int count = 0;
        SoundPlayer NotificationSound;
        System.Speech.Synthesis.SpeechSynthesizer Voice;
        System.Windows.Threading.DispatcherTimer Timer;
        COMMON.DataEvent MainEvent;

        private BOL.Display_Settings.objDisplaySettings _DisplaySetting;
        public BOL.Display_Settings.objDisplaySettings DisplaySetting
        {
            get
            {
                return _DisplaySetting;
            }
            set
            {
                if (_DisplaySetting != value)
                {
                    _DisplaySetting = value;
                    OnPropertyChanged("DisplaySetting");
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
                    OnPropertyChanged("Title_Color");
                    OnPropertyChanged("Number_Color");
                    OnPropertyChanged("Background_Color");
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

        public Brush Title_Color
        {
            get
            {
                var bc = new BrushConverter();
                return (Brush)bc.ConvertFromString(_Terminal.title_color);
            }

        }

        public Brush Number_Color
        {
            get
            {
                var bc = new BrushConverter();
                return (Brush)bc.ConvertFromString(_Terminal.number_color);
            }
        }

        public Brush Background_Color
        {
            get
            {
                var bc = new BrushConverter();
                return (Brush)bc.ConvertFromString(_Terminal.background_color);
            }
        }

        private Visibility _IsVisible;
        public Visibility IsVisible
        {
            get
            {
                return _IsVisible;
            }
            set
            {
                if (_IsVisible != value)
                {
                    _IsVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }
        #endregion 

        #region Methods
        void Initialize()
        {
            try
            {
                DisplaySetting = DAL.Dislay_Settings.DisplaySettings_Repository.GetCurrentDisplaySettings();
                TerminalQueue = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this.Terminal);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Display", ex.Message, EventLogEntryType.Error);

            }
        }

        void Update_Current_TerminalQueue(bool announce)
        {
            try
            {
                TerminalQueue = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this.Terminal);

                if (announce)
                {
                    if (TerminalQueue.id > 0)
                    {
                        count = 0;
                        Timer.Start();
                        Anounce_Message("Client number " + TerminalQueue.ToString() + ", please proceed to " + Terminal.name);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Display", ex.Message, EventLogEntryType.Error);

            }
        }

        void MainEvent_OnDataConfirm(object obj)
        {
            if (obj != null)
            {
                if (((BOL.Task.objTask)obj).terminal_id == this.Terminal.id)
                {
                    switch (((BOL.Task.objTask)obj).type)
                    {
                        case BOL.Task.objTask.MessageType.NextQueue:
                            Update_Current_TerminalQueue(true);
                            break;
                        case BOL.Task.objTask.MessageType.Recall:
                            Update_Current_TerminalQueue(true);
                            break;
                        case BOL.Task.objTask.MessageType.QueueForward:
                            Update_Current_TerminalQueue(true);
                            break;
                        case BOL.Task.objTask.MessageType.RefreshTerminal:
                            Update_Current_TerminalQueue(true);
                            break;
                        case BOL.Task.objTask.MessageType.CurrentIsSet:
                            Update_Current_TerminalQueue(false);
                            break;
                        default:
                            //nothing
                            break;
                    };
                }
                else
                {
                    switch (((BOL.Task.objTask)obj).type)
                    {
                        case BOL.Task.objTask.MessageType.Unbind:
                            Unbind_MainEvent();
                            break;
                        default:
                            //nothing
                            break;
                    };
                }
            }
        }
        
        void Animate_Notification()
        {
            if (count <= 2)
            {
                if (IsVisible == System.Windows.Visibility.Visible)
                {
                    IsVisible = System.Windows.Visibility.Hidden;
                    count++;
                }
                else
                {
                    IsVisible = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                Timer.Stop();
                count = 0;
                IsVisible = Visibility.Visible;
            }

        }

        void Anounce_Message(string msg)
        {
            MainEvent.ConfirmData(new BOL.Task.objTask() { id = 0, terminal_id = null, type = BOL.Task.objTask.MessageType.Mute });
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                try
                {
                    NotificationSound.PlaySync();
                    Voice.Speak(msg);
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Display", ex.Message, EventLogEntryType.Error);

                }
            };
            bw.RunWorkerCompleted += (s, arg) =>
            {
                if (Voice.State != System.Speech.Synthesis.SynthesizerState.Speaking)
                {
                    MainEvent.ConfirmData(new BOL.Task.objTask() { id = 0, terminal_id = null, type = BOL.Task.objTask.MessageType.Unmute });
                }
            };
            bw.RunWorkerAsync();
        }

        public void Unbind_MainEvent()
        {
            MainEvent.OnDataConfirm -= MainEvent_OnDataConfirm;
        }
        #endregion

        #region Commands
        #endregion
    }
}
