using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Media;
using System.Windows.Controls;
using System.Windows.Media;

namespace NumberQueuingSystem.DISPLAY.Windows.Controls
{
    /// <summary>
    /// Interaction logic for TerminalControl.xaml
    /// </summary>
    public partial class TerminalControl : UserControl
    {
        #region Properties
        public COMMON.DataEvent MainEvent;
        private SoundPlayer NotificationSound;
        private System.Speech.Synthesis.SpeechSynthesizer Voice;
        private BOL.Terminal.objTerminal Terminal;
        private BOL.Terminal_Queue.objTerminalQueue _Queue; 
        public BOL.Terminal_Queue.objTerminalQueue Queue
        {
            get
            {
                return _Queue;
            }
            set
            {
                _Queue = value;
            }
        }
        private System.Windows.Threading.DispatcherTimer Timer;

        #endregion

        #region Constructors
        public TerminalControl()
        {
            InitializeComponent();
        }

        public TerminalControl(COMMON.DataEvent main_event, BOL.Terminal.objTerminal terminal, BOL.Display_Settings.objDisplaySettings display_setting)
        {
            InitializeComponent();

            this.Width = display_setting.terminal_width;
            this.Height = display_setting.terminal_height;
            this.TerminalTitle.FontSize = display_setting.terminal_name_fontsize;
            this.lblQueueNumber.FontSize = display_setting.queue_number_fontsize;
            this.Voice = new System.Speech.Synthesis.SpeechSynthesizer()
            {
                Volume = 100
            };
            this.MainEvent = main_event;
            this.Terminal = terminal;
            this.DefaultFontsize = lblQueueNumber.FontSize;
            this.Timer = new System.Windows.Threading.DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            this.NotificationSound = new SoundPlayer(Properties.Resources.doorbell_sound);
            this.Timer.Tick += Timer_Tick;
            this.MainEvent.OnDataConfirm += MainEvent_OnDataConfirm;

            Initialize_Window();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            Animate_Notification();
        }

        #endregion

        #region Methods
        void Initialize_Window()
        {
           
            BrushConverter convertor = new BrushConverter();
            this.TerminalTitle.Foreground = (Brush)convertor.ConvertFromString(Terminal.title_color);
            this.lblQueueNumber.Foreground = (Brush)convertor.ConvertFromString(Terminal.number_color);
            this.TerminalBackground.Background = (Brush)convertor.ConvertFromString(Terminal.background_color);
            TerminalTitle.Text = this.Terminal.name;
            lblQueueNumber.Text = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this.Terminal).ToString();
        }

        public void Remove_Handler()
        {
            MainEvent.OnDataConfirm -= MainEvent_OnDataConfirm;
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
                            DAL.Task.Task_Repository.Delete_Task((BOL.Task.objTask)obj);
                            Queue = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this.Terminal);
                            lblQueueNumber.Text = Queue.ToString();
                            if (Queue.id > 0)
                            {
                                count = 0;
                                Timer.Start();
                                Anounce_Message("Client number " + Queue.ToString() + ", please proceed to " + Terminal.name);
                            }
                            break;
                        case BOL.Task.objTask.MessageType.Recall:
                            DAL.Task.Task_Repository.Delete_Task((BOL.Task.objTask)obj);
                            Queue = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this.Terminal);
                            lblQueueNumber.Text = Queue.ToString();
                            if (Queue.id > 0)
                            {
                                count = 0;
                                Timer.Start();
                                Anounce_Message("Client number " + Queue.ToString() + ", please proceed to " + Terminal.name);
                            }
                            break;
                        case BOL.Task.objTask.MessageType.QueueForward:
                            DAL.Task.Task_Repository.Delete_Task((BOL.Task.objTask)obj);
                            Queue = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this.Terminal);
                            lblQueueNumber.Text = Queue.ToString();
                            break;
                        case BOL.Task.objTask.MessageType.RefreshTerminal:
                            DAL.Task.Task_Repository.Delete_Task((BOL.Task.objTask)obj);
                            Queue = DAL.Terminal_Queue.TerminalQueue_Repository.GetCurrentQueue(this.Terminal);
                            lblQueueNumber.Text = Queue.ToString();
                            break;
                        default:
                            //nothing
                            break;
                    };
                }
            }
        }

        int count = 0;
        double DefaultFontsize;
        void Animate_Notification()
        {

            if (count <= 2)
            {

                if (lblQueueNumber.Visibility == System.Windows.Visibility.Visible)
                {
                    lblQueueNumber.Visibility = System.Windows.Visibility.Hidden;
                    count++;
                }
                else
                {
                    lblQueueNumber.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                Timer.Stop();
                count = 0;
                lblQueueNumber.Visibility = System.Windows.Visibility.Visible;
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
        #endregion

    }
}
