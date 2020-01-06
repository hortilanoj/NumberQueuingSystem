using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;
using System.Diagnostics;

namespace NumberQueuingSystem.DISPLAY.ViewModels
{
    class vmHeader : INotifyPropertyChanged
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
        #endregion

        #region Constructors
        public vmHeader(COMMON.DataEvent main_event)
        {
            MainEvent = main_event;
            System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 1),
                IsEnabled = true
            };
            Timer.Tick += (s, a) =>
            {
                Current_Date = DateTime.Now.ToLongDateString();
                Current_Time = DateTime.Now.ToLongTimeString();
            };
            Timer.Start();

            MainEvent.OnDataConfirm += MainEvent_OnDataConfirm;
            Update_Display_Setting();
        }

        void MainEvent_OnDataConfirm(object obj)
        {
            if (obj != null)
            {
                switch (((BOL.Task.objTask)obj).type)
                {
                    case BOL.Task.objTask.MessageType.LoadTerminals:
                        Update_Display_Setting();
                        break;
                    default:
                        //nothing
                        break;
                };
            }
        }
        #endregion

        #region Properties
        COMMON.DataEvent MainEvent { get; set; }
        private string _Current_Date;
        public string Current_Date
        {
            get
            {
                return _Current_Date;
            }
            set
            {
                if (_Current_Date != value)
                {
                    _Current_Date = value;
                    OnPropertyChanged("Current_Date");
                }
            }
        }

        private string _Current_Time;
        public string Current_Time
        {
            get
            {
                return _Current_Time;
            }
            set
            {
                if (_Current_Time != value)
                {
                    _Current_Time = value;
                    OnPropertyChanged("Current_Time");
                }
            }
        }

        private BOL.Display_Settings.objDisplaySettings _DisplaySetting;
        public BOL.Display_Settings.objDisplaySettings DisplaySetting
        {
            get
            {
                return _DisplaySetting;
            }
            set
            {
                if (_DisplaySetting != value)
                {
                    _DisplaySetting = value;
                    OnPropertyChanged("DisplaySetting");
                    OnPropertyChanged("Background_Color");
                    OnPropertyChanged("Title_Color");
                    OnPropertyChanged("Date_Color");
                    OnPropertyChanged("Time_Color");
                }
            }
        }

        public Brush Background_Color
        {
            get
            {
                var bc = new BrushConverter();
                return (Brush)bc.ConvertFromString(DisplaySetting.header_background_color);
            }
        }

        public Brush Title_Color
        {
            get
            {
                var bc = new BrushConverter();
                return (Brush)bc.ConvertFromString(DisplaySetting.header_title_color);
            }
        }

        public Brush Date_Color
        {
            get
            {
                var bc = new BrushConverter();
                return (Brush)bc.ConvertFromString(DisplaySetting.header_date_color);
            }
        }

        public Brush Time_Color
        {
            get
            {
                var bc = new BrushConverter();
                return (Brush)bc.ConvertFromString(DisplaySetting.header_time_color);
            }
        }
        #endregion

        #region Methods
        void Update_Display_Setting()
        {
            try
            {
                DisplaySetting = DAL.Dislay_Settings.DisplaySettings_Repository.GetCurrentDisplaySettings();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Display", ex.Message, EventLogEntryType.Error);

            }
        }
        #endregion

        #region Commands
        #endregion
    }
}
