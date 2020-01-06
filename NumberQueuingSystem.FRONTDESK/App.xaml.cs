using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace NumberQueuingSystem.FRONTDESK
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            App.Current.Dispatcher.UnhandledException += Dispatcher_UnhandledException;

            if (NumberQueuingSystem.FRONTDESK.Properties.Settings.Default.IsConfigured)
            {
                DAL.Base.ConnectionManager.ConnectionString = NumberQueuingSystem.FRONTDESK.Properties.Settings.Default.ConnectionString;

                Views.vFrontDesk FrontDeskv = new Views.vFrontDesk();
                ViewModels.vmFrontDesk FrontDeskvm = new ViewModels.vmFrontDesk();
                FrontDeskv.DataContext = FrontDeskvm;
                FrontDeskv.Show();
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
