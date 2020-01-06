using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace NumberQueuingSystem.TERMINAL.ViewModels
{
    class vmTerminalSelection : INotifyPropertyChanged
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
        public vmTerminalSelection()
        {
            Terminals = new ObservableCollection<BOL.Terminal.objTerminal>();

            this.Next_Command = new COMMON.RelayCommand(this.Execute_Next, this.CanNext);
            this.Cancel_Command = new COMMON.RelayCommand(this.Execute_Cancel, this.CanCancel);

            Update_Terminals();
        }
        #endregion

        #region Properties
        private ObservableCollection<BOL.Terminal.objTerminal> _Terminals;
        public ObservableCollection<BOL.Terminal.objTerminal> Terminals
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
                    OnPropertyChanged("Next_Enabled");
                }
            }
        }

        public bool Next_Enabled
        {
            get
            {
                return (Terminal != null ? true : false);
            }
        }
        #endregion

        #region Methods
        void Update_Terminals()
        {
            Terminals.Clear();
            List<BOL.Terminal.objTerminal> List = DAL.Terminal.Terminal_Repository.GetActiveTerminals();
            List.ForEach(x => Terminals.Add(x));
        }
        void TerminalV_Activated(object sender, System.EventArgs e)
        {
            ((Views.vTerminal)sender).Top = System.Windows.SystemParameters.WorkArea.Height - ((Views.vTerminal)sender).Height;
            ((Views.vTerminal)sender).Left = System.Windows.SystemParameters.WorkArea.Width - ((Views.vTerminal)sender).Width;
        }
        #endregion

        #region Commands
        public COMMON.RelayCommand Next_Command { get; private set; }
        void Execute_Next(object args)
        {
            Properties.Settings.Default.TerminalId = Terminal.id;
            Properties.Settings.Default.IsConfigured = true;
            Properties.Settings.Default.Save();

            App.Current.Shutdown();
        }
        bool CanNext(object args)
        {
            return true;
        }

        public COMMON.RelayCommand Cancel_Command { get; private set; }
        void Execute_Cancel(object args)
        {
            CloseAction();
        }
        bool CanCancel(object args)
        {
            return true;
        }
        #endregion
    }
}
