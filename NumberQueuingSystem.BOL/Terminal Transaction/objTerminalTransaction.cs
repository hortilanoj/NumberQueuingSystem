using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberQueuingSystem.BOL.Terminal_Transaction
{
    public class objTerminalTransaction
    {
        #region Poperties
        public long? id { get; set; }
        public long? terminal_id { get; set; }
        public string Transaction
        {
            get
            {
                return objTransaction.name;
            }
        }
        public byte? priority_level { get; set; }
        #endregion

        #region Objects
        public BOL.Transaction.objTransaction objTransaction { get; set; }
        #endregion

        #region Constructors
        public objTerminalTransaction()
        {
            id = 0;
            terminal_id = 0;
            objTransaction = new Transaction.objTransaction();
            priority_level = 0;
        }
        #endregion
    }
}
