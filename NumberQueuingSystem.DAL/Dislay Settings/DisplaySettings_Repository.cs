using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace NumberQueuingSystem.DAL.Dislay_Settings
{
    public static class DisplaySettings_Repository
    {
        public static BOL.Display_Settings.objDisplaySettings GetCurrentDisplaySettings()
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "* " +
                                "FROM " +
                                        "display_settings " +
                                "ORDER BY " +
                                        "id DESC " +
                                "LIMIT 1";

                BOL.Display_Settings.objDisplaySettings Current = conn.Query<BOL.Display_Settings.objDisplaySettings>(statement).SingleOrDefault();
                conn.Close();
                conn.Dispose();

                if (Current == null)
                    Current = new BOL.Display_Settings.objDisplaySettings();

                return Current;
            }
        }

        public static bool AddNew_DisplaySetting(BOL.Display_Settings.objDisplaySettings display_setting)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "INSERT INTO " +
                                        "display_settings " +
                                "(terminal_name_fontsize, queue_number_fontsize, terminal_width, terminal_height, header_background_color, header_title_color, header_title_fontsize, header_date_color, header_date_fontsize, header_time_color, header_time_fontsize) " +
                                "VALUES " +
                                "(@terminal_name_fontsize, @queue_number_fontsize, @terminal_width, @terminal_height, @header_background_color, @header_title_color, @header_title_fontsize, @header_date_color, @header_date_fontsize, @header_time_color, @header_time_fontsize)";

                conn.Execute(statement,
                    new
                    {
                        terminal_name_fontsize = display_setting.terminal_name_fontsize,
                        queue_number_fontsize = display_setting.queue_number_fontsize,
                        terminal_width = display_setting.terminal_width,
                        terminal_height = display_setting.terminal_height,
                        header_background_color = display_setting.header_background_color,
                        header_title_color = display_setting.header_title_color,
                        header_title_fontsize = display_setting.header_title_fontsize,
                        header_date_color = display_setting.header_date_color,
                        header_date_fontsize = display_setting.header_date_fontsize,
                        header_time_color = display_setting.header_time_color,
                        header_time_fontsize = display_setting.header_time_fontsize
                    });
                conn.Close();
                conn.Dispose();

                return true;
            }
        }
    }
}
