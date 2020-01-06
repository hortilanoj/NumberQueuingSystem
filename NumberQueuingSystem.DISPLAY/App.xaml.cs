using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace NumberQueuingSystem.DISPLAY
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
            if (NumberQueuingSystem.DISPLAY.Properties.Settings.Default.IsConfigured)
            {
                DAL.Base.ConnectionManager.ConnectionString = NumberQueuingSystem.DISPLAY.Properties.Settings.Default.ConnectionString;

                Views.vDisplay DisplayV = new Views.vDisplay();
                ViewModels.vmDisplay DisplayVM = new ViewModels.vmDisplay();

                DisplayV.MouseRightButtonDown += (s, a) =>
                    {
                        ContextMenu Menus = new ContextMenu();

                        MenuItem menuPlayNextRandomMedia = new MenuItem() { Header = "Play Next Random Media" };
                        menuPlayNextRandomMedia.Click += (s1, a1) => { DisplayVM.Load_New_Media(); };
                        Menus.Items.Add(menuPlayNextRandomMedia);

                        MenuItem menuOpenFileLocation = new MenuItem() { Header = "Open File Location" };
                        menuOpenFileLocation.Click += (s2, a2) => { System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.BaseDirectory, "explorer.exe"); };
                        Menus.Items.Add(menuOpenFileLocation);

                        MenuItem menuWindow = new MenuItem() { Header = "Window Mode" };
                        menuWindow.Click += (s3, a3) =>
                            {
                                DisplayV.ResizeMode = System.Windows.ResizeMode.CanResize;
                                DisplayV.WindowState = System.Windows.WindowState.Normal;
                                DisplayV.WindowStyle = System.Windows.WindowStyle.ToolWindow;
                            };
                        Menus.Items.Add(menuWindow);

                        MenuItem menuFullScreen = new MenuItem() { Header = "Fullscreen" };
                        menuFullScreen.Click += (s4, a4) =>
                            {
                                DisplayV.ResizeMode = System.Windows.ResizeMode.CanResize;
                                DisplayV.WindowState = System.Windows.WindowState.Maximized;
                                DisplayV.WindowStyle = System.Windows.WindowStyle.None;
                            };
                        Menus.Items.Add(menuFullScreen);

                        MenuItem menuSetup = new MenuItem() { Header = "Setup" };
                        menuSetup.Click += (s5, a5) =>
                            {
                                Views.vConnection ConnectionV = new Views.vConnection();
                                ViewModels.vmConnection ConnectionVM = new ViewModels.vmConnection();
                                ConnectionV.DataContext = ConnectionVM;
                                ConnectionVM.CloseAction = new Action(() =>
                                {
                                    ConnectionV.Close();
                                });
                                ConnectionV.ShowDialog();
                            };
                        Menus.Items.Add(menuSetup);

                        MenuItem menuExit = new MenuItem() { Header = "Exit" };
                        menuExit.Click += (s6, a6) => { App.Current.Shutdown(); };
                        Menus.Items.Add(menuExit);

                        Menus.IsOpen = true;
                    };

                DisplayV.DataContext = DisplayVM;
                DisplayV.Show();
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
