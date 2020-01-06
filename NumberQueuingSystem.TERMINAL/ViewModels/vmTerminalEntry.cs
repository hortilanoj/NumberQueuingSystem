using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;

namespace NumberQueuingSystem.TERMINAL.ViewModels
{
    class vmTerminalEntry : INotifyPropertyChanged
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

        #region Constructors
        public vmTerminalEntry(COMMON.SaveMode.Mode mode, BOL.Terminal.objTerminal terminal)
        {
            this.Mode = mode;
            this.Terminal = terminal;
            this.Save_Command = new COMMON.RelayCommand(Execute_Save, CanSave);
            this.Cancel_Command = new COMMON.RelayCommand(Execute_Cancel, CanCancel);

            Convert_StringToBrush();
        }
        #endregion

        #region Methods
        void Convert_StringToBrush()
        {
            Title_Color = (Color)ColorConverter.ConvertFromString(Terminal.title_color);
            Number_Color = (Color)ColorConverter.ConvertFromString(Terminal.number_color);
            Background_Color = (Color)ColorConverter.ConvertFromString(Terminal.background_color);
        }
        #endregion

        #region Properties

        private COMMON.SaveMode.Mode Mode;
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

        public string Name
        {
            get
            {
                return _Terminal.name;
            }
            set
            {
                if (_Terminal.name != value)
                {
                    _Terminal.name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Description
        {
            get
            {
                return _Terminal.description;
            }
            set
            {
                if (_Terminal.description != value)
                {
                    _Terminal.description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        public bool Active
        {
            get
            {
                return _Terminal.active;
            }
            set
            {
                if (_Terminal.active != value)
                {
                    _Terminal.active = value;
                    OnPropertyChanged("Active");
                }
            }
        }

        private Color _Title_Color;
        public Color Title_Color
        {
            get
            {
                return _Title_Color;
            }
            set
            {
                if (_Title_Color != value)
                {
                    _Title_Color = value;
                    OnPropertyChanged("Title_Color");
                    Terminal.title_color = value.ToString();
                    OnPropertyChanged("Terminal");
                }
            }
        }

        private Color _Number_Color;
        public Color Number_Color
        {
            get
            {
                return _Number_Color;
            }
            set
            {
                if (_Number_Color != value)
                {
                    _Number_Color = value;
                    OnPropertyChanged("Number_Color");
                    Terminal.number_color = value.ToString();
                    OnPropertyChanged("Terminal");
                }
            }
        }

        private Color _Background_Color;
        public Color Background_Color
        {
            get
            {
                return _Background_Color;
            }
            set
            {
                if (_Background_Color != value)
                {
                    _Background_Color = value;
                    OnPropertyChanged("Background_Color");
                    Terminal.background_color = value.ToString();
                    OnPropertyChanged("Terminal");
                }
            }
        }
        #endregion

        #region Command
        public COMMON.RelayCommand Cancel_Command { get; private set; }

        void Execute_Cancel(object para)
        {
            CloseAction();
        }

        bool CanCancel(object para)
        {
            return true;
        }

        public COMMON.RelayCommand Save_Command { get; private set; }

        void Execute_Save(object para)
        {
            try
            {
                if (this.Mode == COMMON.SaveMode.Mode.AddNew)
                {
                    if (DAL.Terminal.Terminal_Repository.Add(Terminal))
                    {
                        DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = null, type = BOL.Task.objTask.MessageType.LoadTerminals });
                        CloseAction();
                    }
                }
                else
                {
                    if (DAL.Terminal.Terminal_Repository.Update(Terminal))
                    {
                        DAL.Task.Task_Repository.AddNew_Task(new BOL.Task.objTask() { terminal_id = null, type = BOL.Task.objTask.MessageType.LoadTerminals });
                        CloseAction();
                    }
                }
                
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Terminal", ex.Message, EventLogEntryType.Error);
            }
           
        }
        bool CanSave(object para)
        {
            return true;
        }
        #endregion

    }
}
