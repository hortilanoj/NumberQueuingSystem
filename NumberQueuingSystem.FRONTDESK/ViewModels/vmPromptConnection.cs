using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace NumberQueuingSystem.FRONTDESK.ViewModels
{
    class vmPromptConnection : INotifyPropertyChanged
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

        public vmPromptConnection()
        {
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
                    if ((bool)arg.Result)
                    {
                        CloseAction();
                    }
                    else
                    {
                        Check_Connection();
                    }
                }
                else
                {
                    Check_Connection();
                }
            };
            bw.RunWorkerAsync();
        }


    }
}
