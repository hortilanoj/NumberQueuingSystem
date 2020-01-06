using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Configuration;

namespace NumberQueuingSystem.TERMINAL.ViewModels
{
    class vmConnection : INotifyPropertyChanged
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
        public Action CloseAction { get; set; }
        #endregion

        #region Constructors
        public vmConnection()
        {
            Connection_String = Properties.Settings.Default.ConnectionString;
            DAL.Base.ConnectionManager.ConnectionString = Connection_String;

            this.Next_Command = new COMMON.RelayCommand(this.Execute_Next, this.CanNext);
            this.Cancel_Command = new COMMON.RelayCommand(this.Execute_Cancel, this.CanCancel);
        }
        #endregion

        #region Properties
        private string _Connection_String;
        public string Connection_String
        {
            get
            {
                return _Connection_String;
            }
            set
            {
                if (_Connection_String != value)
                {
                    _Connection_String = value;
                    OnPropertyChanged("Connection_String");
                }
            }
        }
        #endregion

        #region Methods
        #endregion

        #region Commands
        public COMMON.RelayCommand Next_Command { get; private set; }
        void Execute_Next(object para)
        {
            if (DAL.Base.ConnectionManager.Check_Connection(Connection_String))
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
                connectionStringsSection.ConnectionStrings["NumberQueuingSystem.TERMINAL.Properties.Settings.ConnectionString"].ConnectionString = Connection_String;
                config.Save();
                ConfigurationManager.RefreshSection("connectionStrings");

                DAL.Base.ConnectionManager.ConnectionString = Connection_String;

                Views.vTerminalSelection TerminalSelectionV = new Views.vTerminalSelection();
                ViewModels.vmTerminalSelection TerminalSelectionVM = new vmTerminalSelection();
                TerminalSelectionV.DataContext = TerminalSelectionVM;
                TerminalSelectionVM.CloseAction = new Action(() => 
                {
                    TerminalSelectionV.Close();
                });
                TerminalSelectionV.Show();

                //CloseAction();
            }
            else
            {
                System.Windows.MessageBox.Show("Cannot connect to server, check the connection parameters and try again", "UNABLE TO CONTINUE", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        bool CanNext(object para)
        {
            return true;
        }

        public COMMON.RelayCommand Cancel_Command { get; private set; }
        void Execute_Cancel(object para)
        {
            CloseAction();
        }
        bool CanCancel(object para)
        {
            return true;
        }
        #endregion
    }
}
