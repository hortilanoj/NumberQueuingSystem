using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace NumberQueuingSystem.DISPLAY.ViewModels
{
    class vmDisplay : INotifyPropertyChanged
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
        //public Action Stop_Action { get; set; }
        //public Action Play_Action { get; set; }
        //public Action Pause_Action { get; set; }
        //public Action Mute_Action { get; set; }
        //public Action UnMute_Action { get; set; }
        #endregion

        #region Construtors
        public vmDisplay()
        {
            Initialize();
        }
        #endregion

        #region Properties
        private COMMON.DataEvent _MainEvent;
        public COMMON.DataEvent MainEvent
        {
            get
            {
                return _MainEvent;
            }
            set
            {
                if (_MainEvent != value)
                {
                    _MainEvent = value;
                    OnPropertyChanged("MainEvent");
                }
            }
        }

        private Views.vHeader _Header;
        public Views.vHeader Header
        {
            get
            {
                return _Header;
            }
            set
            {
                if (_Header != value)
                {
                    _Header = value;
                    OnPropertyChanged("Header");
                }
            }
        }

        private MediaElement _MediaControl;
        public MediaElement MediaControl
        {
            get
            {
                return _MediaControl;
            }
            set
            {
                if (_MediaControl != value)
                {
                    _MediaControl = value;
                    OnPropertyChanged("MediaControl");
                }
            }
        }

        private ObservableCollection<Views.vTerminal> _Terminals;
        public ObservableCollection<Views.vTerminal> Terminals
        {
            get
            {
                return _Terminals;
            }
            set
            {
                if (_Terminals != value)
                {
                    _Terminals = value;
                    OnPropertyChanged("Terminals");
                }
            }
        }

        #endregion

        #region Methods
        void Initialize()
        {
            MainEvent = new COMMON.DataEvent();
            Terminals = new ObservableCollection<Views.vTerminal>();

            Header = new Views.vHeader();
            ViewModels.vmHeader HeaderVM = new vmHeader(MainEvent);
            Header.DataContext = HeaderVM;
            
            MainEvent.OnDataConfirm += MainEvent_OnDataConfirm;

            Initialize_MediaControl();

            Check_Connection();

        }

        void Initialize_MediaControl()
        {
            MediaControl = new MediaElement()
            {
                Volume = 0.29,
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual,
                Stretch = System.Windows.Media.Stretch.Uniform,
                StretchDirection = StretchDirection.Both
            };

            MediaControl.MediaEnded += MediaControl_MediaEnded;
            MediaControl.MediaFailed += MediaControl_MediaFailed;
            MediaControl.MediaOpened += MediaControl_MediaOpened;

            Load_New_Media();
        }


        void MediaControl_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            
            if (!MediaControl.NaturalDuration.HasTimeSpan)
            {

                MediaControl.Pause();

                BackgroundWorker bwImageDelay = new BackgroundWorker() { WorkerSupportsCancellation = true };
                bwImageDelay.DoWork += (bs, barg) =>
                {
                    System.Threading.Thread.Sleep(new TimeSpan(0, 0, 30));
                };

                bwImageDelay.RunWorkerCompleted += (bs, barg) =>
                {
                    if (!MediaControl.NaturalDuration.HasTimeSpan)
                    {
                        Load_New_Media();
                    }
                };
                bwImageDelay.RunWorkerAsync();
            }
        }

        void MediaControl_MediaFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            Load_New_Media();
        }

        void MediaControl_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            Load_New_Media();
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
                Clear_All_Tasks();
                Update_Terminal_List();
                Start_Query();
            };
            bw.RunWorkerAsync();
        }

        void Clear_All_Tasks()
        {
            try
            {
                DAL.Task.Task_Repository.Delete_All_Task();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Display", ex.Message, EventLogEntryType.Error);
            }
        }

        void Start_Query()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                try
                {
                    arg.Result = DAL.Task.Task_Repository.GetDisplayCurrentTask();
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Display", ex.Message, EventLogEntryType.Error);
                }
                
            };

            bw.RunWorkerCompleted += (s, arg) =>
            {
                if (arg.Error == null)
                {
                    if (arg.Result != null)
                    {
                        MainEvent.ConfirmData((BOL.Task.objTask)arg.Result);
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
            ViewModels.vmPromptConnection PromptVM = new ViewModels.vmPromptConnection();
            PromptVM.CloseAction = new Action(() => { PromtV.Close(); });
            PromtV.DataContext = PromptVM;
            PromtV.ShowDialog();
        }

        string getrandomfile2(string path)
        {
            string file = null;
            if (!string.IsNullOrEmpty(path))
            {
                while (string.IsNullOrEmpty(file))
                {
                    var extensions = new string[] { ".png", ".jpg", ".mpeg", ".mp4", ".avi", ".mov", ".bmp", ".jpeg", ".mpeg2" };
                    try
                    {
                        var di = new DirectoryInfo(path);
                        var rgFiles = di.GetFiles("*.*").Where(f => extensions.Contains(f.Extension.ToLower()));
                        Random R = new Random();
                        file = rgFiles.ElementAt(R.Next(0, rgFiles.Count())).FullName;
                    }
                    catch
                    {
                        Load_New_Media();
                    }

                }
            }
            return file;
        }

        public void Load_New_Media()
        {
            try
            {
                MediaControl.Stop();
                MediaControl.Source  = new Uri(getrandomfile2(@"C:\Queuing System\Media\"));
                MediaControl.Play();
            }
            catch
            {
                Load_New_Media();
            }
        }

        void Update_Terminal_List()
        {
            MainEvent.ConfirmData(new BOL.Task.objTask() { id = null, terminal_id = null, type = BOL.Task.objTask.MessageType.Unbind });
            List<BOL.Terminal.objTerminal> List = DAL.Terminal.Terminal_Repository.GetSortedActiveTerminals();
            Terminals.Clear();
            List.ForEach(x =>
                {
                    Views.vTerminal TerminalV = new Views.vTerminal();
                    ViewModels.vmTerminal TerminalVM = new ViewModels.vmTerminal(x, MainEvent);
                    TerminalV.DataContext = TerminalVM;
                    Terminals.Add(TerminalV);
                });
        }

        void MainEvent_OnDataConfirm(object obj)
        {
            if (obj != null)
            {
                if (((BOL.Task.objTask)obj).terminal_id == null)
                {
                    switch (((BOL.Task.objTask)obj).type)
                    {
                        case BOL.Task.objTask.MessageType.ChangeMedia:
                            Load_New_Media();
                            break;
                        case BOL.Task.objTask.MessageType.Play:
                            MediaControl.Play();
                            break;
                        case BOL.Task.objTask.MessageType.Pause:
                            MediaControl.Pause();
                            break;
                        case BOL.Task.objTask.MessageType.Mute:
                            MediaControl.IsMuted = true;
                            break;
                        case BOL.Task.objTask.MessageType.Unmute:
                            MediaControl.IsMuted = false;
                            break;
                        case BOL.Task.objTask.MessageType.LoadTerminals:
                            Update_Terminal_List();
                            break;
                        default:
                            //nothing
                            break;
                    };
                }
            }
        }

        #endregion

        #region Commands
        #endregion
    }
}
