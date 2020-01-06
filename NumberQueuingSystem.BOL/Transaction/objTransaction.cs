
namespace NumberQueuingSystem.BOL.Transaction
{
    public class objTransaction
    {
        #region Properties
        public long? id { get; set; }
        public string name { get; set; }
        public string prefix { get; set; }
        public string description { get; set; }
        public bool active { get; set; }
        #endregion

        #region Constructors
        public objTransaction()
        {
            id = 0;
            name = "";
            prefix = "";
            description = "";
            active = false;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return name;
        }
        #endregion
    }
}
