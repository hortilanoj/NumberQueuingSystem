using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NumberQueuingSystem.FRONTDESK.Views
{
    /// <summary>
    /// Interaction logic for vFrontDesk.xaml
    /// </summary>
    public partial class vFrontDesk : Window
    {
        public vFrontDesk()
        {
            InitializeComponent();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu Menus = new ContextMenu();

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

            MenuItem menuPrinting = new MenuItem()
            {
                Header = (Properties.Settings.Default.EnablePrinting ? "Disable Printing" : "Enable Printing")
            };
            menuPrinting.Click += menuPrinting_Click;
            Menus.Items.Add(menuPrinting);

            MenuItem menuExit = new MenuItem()
            {
                Header = "Exit"
            };
            menuExit.Click += menuExit_Click;
            Menus.Items.Add(menuExit);

            Menus.IsOpen = true;
        }

        void menuPrinting_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.EnablePrinting = !Properties.Settings.Default.EnablePrinting;
            Properties.Settings.Default.Save();
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
            ConnectionV.ShowDialog();
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

        void menuOpenFileLocation_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.BaseDirectory, "explorer.exe");
        }

        void menuExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.InvokeShutdown();
        }
    }
}
