using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace NumberQueuingSystem.DISPLAY.Windows
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        System.Windows.Threading.DispatcherTimer Timer;
        COMMON.DataEvent MainEvent;
        public Main()
        {
                InitializeComponent();
                Initialize_Window();
                Initialize_Process();
                Load_New_Media();
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
                Load_Terminals();
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

        void Initialize_Window()
        {
            this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            this.Width = System.Windows.SystemParameters.WorkArea.Width;
            this.Height = System.Windows.SystemParameters.WorkArea.Height;
        
            MediaControl.Width = (System.Windows.SystemParameters.WorkArea.Width / 5) * 3.5;
            TerminalPanel.Width = (System.Windows.SystemParameters.WorkArea.Width / 5) *1.5;
            TitleGrid.Width = (System.Windows.SystemParameters.WorkArea.Width / 5) * 1.5;
        }

        void Start_Query()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                    arg.Result = DAL.Task.Task_Repository.GetDisplayCurrentTask();
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

        void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            EventLog.WriteEntry("Display", e.Exception.Message, EventLogEntryType.Error);
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
                            DAL.Task.Task_Repository.Delete_Task((BOL.Task.objTask)obj);
                            Load_New_Media();
                            break;
                        case BOL.Task.objTask.MessageType.Play:

                            break;
                        case BOL.Task.objTask.MessageType.Pause:

                            break;
                        case BOL.Task.objTask.MessageType.Mute:
                            MediaControl.IsMuted = true;
                            break;
                        case BOL.Task.objTask.MessageType.Unmute:
                            MediaControl.IsMuted = false;
                            break;
                        case BOL.Task.objTask.MessageType.LoadTerminals:
                            DAL.Task.Task_Repository.Delete_Task((BOL.Task.objTask)obj);
                            Load_Terminals();
                            break;
                        default:
                            //nothing
                            break;
                    };
                }
            }
        }

        void Initialize_Process()
        {
            MainEvent = new COMMON.DataEvent();
            MainEvent.OnDataConfirm += MainEvent_OnDataConfirm;

            Timer = new System.Windows.Threading.DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 30),
            };
            Timer.Tick += Timer_Tick;

            MediaControl.MediaEnded += MediaControl_MediaEnded;
            MediaControl.MediaFailed += MediaControl_MediaFailed;
            MediaControl.MediaOpened += MediaControl_MediaOpened;
            
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            Load_New_Media();
        }

        private void Load_New_Media()
        {
            MediaControl.Source = new Uri(getrandomfile2(@"C:\Queuing System\Media\"), UriKind.RelativeOrAbsolute);
            MediaControl.Play();
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
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry("Display", ex.Message, EventLogEntryType.Error);
                    }

                }
            }
            return file;
        }


        void Load_Terminals()
        {
            BOL.Display_Settings.objDisplaySettings DisplaySetting = DAL.Dislay_Settings.DisplaySettings_Repository.GetCurrentDisplaySettings();
            
            foreach (Windows.Controls.TerminalControl obj in TerminalPanel.Children)
            {
                obj.Remove_Handler();
            }
            TerminalPanel.Children.Clear();

            List<BOL.Terminal.objTerminal> Terminals = DAL.Terminal.Terminal_Repository.GetActiveTerminals();

            foreach (BOL.Terminal.objTerminal terminal in Terminals)
            {
                TerminalPanel.Children.Add(new Windows.Controls.TerminalControl(this.MainEvent, terminal, DisplaySetting));
            }
        }

        private void MediaControl_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Load_New_Media();
        }

        private void MediaControl_MediaEnded(object sender, RoutedEventArgs e)
        {
            Load_New_Media();
        }

        private void MediaControl_MediaOpened(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
            if (!MediaControl.NaturalDuration.HasTimeSpan)
            {
                Timer.Start();
            }
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu Menus = new ContextMenu();

            MenuItem menuPlayNextRandomMedia = new MenuItem()
            {
                Header = "Play Next Random Media"
            };
            menuPlayNextRandomMedia.Click += menuPlayNextRandomMedia_Click;

            Menus.Items.Add(menuPlayNextRandomMedia);

            MenuItem menuOpenFileLocation = new MenuItem()
            {
                Header = "Open File Location"
            };
            menuOpenFileLocation.Click += menuOpenFileLocation_Click;
            Menus.Items.Add(menuOpenFileLocation);

            MenuItem menuWindow = new MenuItem()
            {
                Header = "Window Mode"
            };
            menuWindow.Click += menuWindow_Click;
            Menus.Items.Add(menuWindow);

            MenuItem menuFullScreen = new MenuItem()
            {
                Header = "Fullscreen"
            };
            menuFullScreen.Click += menuFullScreen_Click;
            Menus.Items.Add(menuFullScreen);

            MenuItem menuSetup = new MenuItem()
            {
                Header = "Setup"
            };
            menuSetup.Click += menuSetup_Click;
            Menus.Items.Add(menuSetup);

            MenuItem menuExit = new MenuItem()
            {
                Header = "Exit"
            };
            menuExit.Click += menuExit_Click;
            Menus.Items.Add(menuExit);

            Menus.IsOpen = true;
        }

        void menuSetup_Click(object sender, RoutedEventArgs e)
        {
            Views.vConnection ConnectionV = new Views.vConnection();
            ViewModels.vmConnection ConnectionVM = new ViewModels.vmConnection();
            ConnectionV.DataContext = ConnectionVM;
            ConnectionVM.CloseAction = new Action(() =>
            {
                ConnectionV.Close();
            });
            ConnectionV.Show();

            this.Close();
        }

        void menuFullScreen_Click(object sender, RoutedEventArgs e)
        {
            this.ResizeMode = System.Windows.ResizeMode.CanResize;
            this.WindowState = System.Windows.WindowState.Maximized;
            this.WindowStyle = System.Windows.WindowStyle.None;
        }

        void menuWindow_Click(object sender, RoutedEventArgs e)
        {
            this.ResizeMode = System.Windows.ResizeMode.CanResize;
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.ToolWindow;
        }

        void menuExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.InvokeShutdown();
        }

        void menuOpenFileLocation_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.BaseDirectory, "explorer.exe");
        }

        void menuPlayNextRandomMedia_Click(object sender, RoutedEventArgs e)
        {
            Load_New_Media();
        }

    }
}
