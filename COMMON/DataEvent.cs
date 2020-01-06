using System;

namespace COMMON
{
    public class DataEvent
    {

        public delegate void OnDataConfirmed(Object obj);

        public void ConfirmData()
        {
            try
            {
                OnDataConfirm.Invoke(null);
            }
            catch (Exception ex)
            {
                throw new InsufficientExecutionStackException(ex.Message);
            }
        }

        public void ConfirmData(Object obj)
        {
            try
            {
                OnDataConfirm.Invoke(obj);
            }
            catch (Exception ex)
            {
                throw new InsufficientExecutionStackException(ex.Message);
            }
        }

        public event OnDataConfirmed OnDataConfirm;

    }
}
