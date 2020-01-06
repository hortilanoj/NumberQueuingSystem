
using System;
using System.Diagnostics;
using System.Windows;

namespace NumberQueuingSystem.TERMINAL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Size TerminalSize { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            App.Current.Dispatcher.UnhandledException += Dispatcher_UnhandledException;


            if (NumberQueuingSystem.TERMINAL.Properties.Settings.Default.IsConfigured)
            {
                Views.vTerminal TerminalV = new Views.vTerminal();
                ViewModels.vmTerminal TerminalVM = new ViewModels.vmTerminal();
                TerminalVM.CloseAction = new Action(() =>
                {
                    TerminalV.Close();
                });
                TerminalV.DataContext = TerminalVM;
                TerminalV.lstWatingList.MouseDoubleClick += (s, a) => { TerminalVM.Show_WaitingList_Options(); };
                TerminalV.lstHistory.MouseDoubleClick += (s, a) => { TerminalVM.Show_History_Options(); };
                TerminalV.Loaded += (s, a) => 
                {
                    TerminalV.Top = System.Windows.SystemParameters.WorkArea.Height - TerminalV.Height;
                    TerminalV.Left = System.Windows.SystemParameters.WorkArea.Width - TerminalV.Width;
                    TerminalSize = new Size(TerminalV.Width, TerminalV.Height);
                };
                TerminalV.Show();
            }
            else
            {
                Views.vConnection ConnectionV = new Views.vConnection();
                ViewModels.vmConnection ConnectionVM = new ViewModels.vmConnection();
                ConnectionV.DataContext = ConnectionVM;
                ConnectionVM.CloseAction = new Action(() =>
                {
                    ConnectionV.Close();
                });
                ConnectionV.Show();
            }

        }

        void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            EventLog.WriteEntry("Terminal", e.Exception.Message, EventLogEntryType.Error);
            e.Handled = true;
        }

        
    }
}
