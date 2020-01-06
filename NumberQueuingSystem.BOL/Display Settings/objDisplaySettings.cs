namespace NumberQueuingSystem.BOL.Display_Settings
{
    public class objDisplaySettings
    {
        #region Propeties
        public int? id { get; set; }
        public double terminal_name_fontsize { get; set; }
        public double queue_number_fontsize { get; set; }
        public double terminal_width { get; set; }
        public double terminal_height { get; set; }
        public string header_background_color { get; set; }
        public string header_title_color { get; set; }
        public double header_title_fontsize { get; set; }
        public string header_date_color { get; set; }
        public double header_date_fontsize { get; set; }
        public string header_time_color { get; set; }
        public double header_time_fontsize { get; set; }
        #endregion

        #region Constuctor
        public objDisplaySettings()
        {
            id = 0;
            terminal_name_fontsize = 30;
            queue_number_fontsize = 60;
            terminal_width = 369.669;
            terminal_height = 187.693;
            header_background_color = "#FF1F00C1";
            header_title_color = "#FFFFFFFF";
            header_title_fontsize = 50;
            header_date_color = "#FFFFFFFF";
            header_date_fontsize = 20;
            header_time_color = "#FFFFFFFF";
            header_time_fontsize = 20;
        }
        #endregion
    }
}
