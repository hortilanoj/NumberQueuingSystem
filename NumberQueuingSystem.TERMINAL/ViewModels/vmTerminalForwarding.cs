using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace NumberQueuingSystem.TERMINAL.ViewModels
{
    class vmTerminalForwarding : INotifyPropertyChanged
    {
        #region Event
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

        #region Properties
        private ObservableCollection<Views.ucButton> _Buttons;
        public ObservableCollection<Views.ucButton> Buttons
        {
            get
            {
                return _Buttons;
            }
            set
            {
                if (_Buttons != value)
                {
                    _Buttons = value;
                    OnPropertyChanged("Buttons");
                }
            }
        }

        private BOL.Terminal_Queue.objTerminalQueue _Queue;
        public BOL.Terminal_Queue.objTerminalQueue Queue
        {
            get
            {
                return _Queue;
            }
            set
            {
                if (_Queue != value)
                {
                    _Queue = value;
                    OnPropertyChanged("Queue");
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
                }
            }
        }
        #endregion

        #region Constructors
        public vmTerminalForwarding(BOL.Terminal.objTerminal terminal, BOL.Terminal_Queue.objTerminalQueue queue)
        {
            this.Terminal = terminal;
            this.Queue = queue;
            Buttons = new ObservableCollection<Views.ucButton>();
            Update_Buttons_List();
        }
        #endregion

        #region Methods
        void Update_Buttons_List()
        {
            List<BOL.Terminal.objTerminal> List = DAL.Terminal.Terminal_Repository.GetTerminalToTransfer(this.Terminal);
            Buttons.Clear();

            foreach(var obj in List)
            {
                Views.ucButton Buttonv = new Views.ucButton();
                ViewModels.ucButton Buttonvm = new ucButton(obj, this.Queue);
                Buttonv.DataContext = Buttonvm;
                Buttonvm.CloseAction = new Action(() =>
                {
                    CloseAction();
                });
                Buttons.Add(Buttonv);
            }

            Views.ucButton ButtonCancelV = new Views.ucButton()
            {
                Content = "CANCEL"
            };
            ButtonCancelV.Click += ButtonCancelV_Click;
            Buttons.Add(ButtonCancelV);

        }

        void ButtonCancelV_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CloseAction();
        }

        #endregion

    }
}
