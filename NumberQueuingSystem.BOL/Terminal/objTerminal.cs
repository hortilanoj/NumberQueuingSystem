
namespace NumberQueuingSystem.BOL.Terminal
{
    public class objTerminal
    {

        #region Properties
        public long id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string title_color { get; set; }
        public string number_color { get; set; }
        public string background_color { get; set; }
        public bool active { get; set; }
        public byte? sorting { get; set; }
        #endregion

        #region Constructors
        public objTerminal()
        {
            id = 0;
            name = "";
            description = "";
            active = true;
            title_color = "#FFFFFFFF";
            number_color = "#FFFFFFFF";
            background_color = "#FF1F00C1";
            sorting = 0;
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
