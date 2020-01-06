using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace NumberQueuingSystem.DAL.Terminal
{
    public static class Terminal_Repository
    {
        public static List<BOL.Terminal.objTerminal> GetAllTerminals()
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "* " +
                                   "FROM " +
                                        "terminal";


                List<BOL.Terminal.objTerminal> List = conn.Query<BOL.Terminal.objTerminal>(statement).ToList();


                conn.Close();
                conn.Dispose();

                return List;
            }
        }

        public static List<BOL.Terminal.objTerminal> GetSortedAllTerminals()
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "* " +
                                "FROM " +
                                        "terminal " +
                                "ORDER BY " +
                                        "sorting ASC";


                List<BOL.Terminal.objTerminal> List = conn.Query<BOL.Terminal.objTerminal>(statement).ToList();


                conn.Close();
                conn.Dispose();

                return List;
            }
        }

        public static List<BOL.Terminal.objTerminal> GetActiveTerminals()
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                    var statement = "SELECT " +
                                            "* " +
                                       "FROM " +
                                            "terminal " +
                                       "WHERE " +
                                            "active = @active_only";


                    List<BOL.Terminal.objTerminal> List = conn.Query<BOL.Terminal.objTerminal>(statement,
                        new
                        {
                            active_only = true
                        }).ToList();


                    conn.Close();
                    conn.Dispose();

                    return List;
           }
        }

        public static List<BOL.Terminal.objTerminal> GetSortedActiveTerminals()
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "* " +
                                   "FROM " +
                                        "terminal " +
                                   "WHERE " +
                                        "active = @active_only " +
                                   "ORDER BY " +
                                        "sorting ASC";


                List<BOL.Terminal.objTerminal> List = conn.Query<BOL.Terminal.objTerminal>(statement,
                    new
                    {
                        active_only = true
                    }).ToList();


                conn.Close();
                conn.Dispose();

                return List;
            }
        }
        public static List<BOL.Terminal.objTerminal> GetTerminalToTransfer(BOL.Terminal.objTerminal terminal)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "* " +
                                   "FROM " +
                                        "terminal " +
                                   "WHERE " +
                                        "active = @active_only AND id != @terminal_id";


                List<BOL.Terminal.objTerminal> List = conn.Query<BOL.Terminal.objTerminal>(statement,
                    new
                    {
                        terminal_id = terminal.id,
                        active_only = true
                    }).ToList();


                conn.Close();
                conn.Dispose();

                return List;
            }
        }


        public static BOL.Terminal.objTerminal GetTerminal(long terminal_id)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "* " +
                                   "FROM " +
                                        "terminal " +
                                   "WHERE " +
                                        "id = @id";


                BOL.Terminal.objTerminal Terminal = conn.Query<BOL.Terminal.objTerminal>(statement,
                    new
                    {
                        id = terminal_id
                    }).SingleOrDefault();

                if (Terminal == null)
                    Terminal = new BOL.Terminal.objTerminal();

                conn.Close();
                conn.Dispose();

                return Terminal;
            }
        }

        public static bool Add(BOL.Terminal.objTerminal terminal)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "SELECT " +
                                       "IFNULL(MAX(sorting), 0) + 1 " +
                               "FROM " +
                                       "terminal";

                int next_sorting = conn.Query<int>(statement).SingleOrDefault();

                statement = "INSERT INTO terminal " +
                                    "(name, description, title_color, number_color, background_color, active, sorting) " +
                                "VALUES " +
                                    "(@name, @description, @title_color, @number_color, @background_color, @active, @sorting)";

                conn.Execute(statement,
                    new
                    {
                        name = terminal.name,
                        description = terminal.description,
                        title_color = terminal.title_color,
                        number_color = terminal.number_color,
                        background_color = terminal.background_color,
                        active = terminal.active,
                        sorting = next_sorting
                    });


                conn.Close();
                conn.Dispose();

                return true;
            }
        }

        public static bool Update(BOL.Terminal.objTerminal terminal)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "UPDATE " +
                                        "terminal " +
                                "SET " +
                                        "name = @name, " +
                                        "description = @description, " +
                                        "title_color = @title_color, " +
                                        "number_color = @number_color, " +
                                        "background_color = @background_color, " +
                                        "active = @active " +
                                "WHERE " +
                                        "id = @id";

                conn.Execute(statement,
                    new
                    {
                        id = terminal.id,
                        name = terminal.name,
                        description = terminal.description,
                        title_color = terminal.title_color,
                        number_color = terminal.number_color,
                        background_color = terminal.background_color,
                        active = terminal.active
                    });


                conn.Close();
                conn.Dispose();

                return true;
            }
        }
           
        public static bool Delete(BOL.Terminal.objTerminal terminal)
        {
             using (var conn = DAL.Base.ConnectionManager.Connection)
             {
                 var statement = "UPDATE " +
                                        "terminal " +
                                "SET " +
                                        "sorting = sorting - 1 " +
                                "WHERE " +
                                       "sorting > @sorting";

                 statement += ";DELETE FROM " +
                                         "terminal " +
                                 "WHERE " +
                                         "id = @id";
                 conn.Execute(statement,
                     new
                     {
                         id = terminal.id,
                         sorting = terminal.sorting
                     });


                 conn.Close();
                 conn.Dispose();

                 return true;
             }      
                
        }

        public static bool Move_Up(BOL.Terminal.objTerminal terminal)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "* " +
                                "FROM " +
                                        "terminal " +
                                "WHERE " +
                                        "sorting < @sorting " +
                                "ORDER BY " +
                                        "sorting DESC " +
                                "LIMIT 1";

                BOL.Terminal.objTerminal previous_terminal = conn.Query<BOL.Terminal.objTerminal>(statement,
                    new
                    {
                        sorting = terminal.sorting
                    }).SingleOrDefault();

                if (previous_terminal != null)
                {
                    statement = "UPDATE " +
                                        "terminal " +
                                "SET " +
                                        "sorting = @current_sorting " +
                                "WHERE " +
                                        "id = @previous_id";
                    statement += ";UPDATE " +
                                        "terminal " +
                                "SET " +
                                        "sorting = @previous_sorting " +
                                "WHERE " +
                                        "id = @current_id";

                    conn.Execute(statement,
                        new
                        {
                            current_sorting = terminal.sorting,
                            previous_id = previous_terminal.id,
                            previous_sorting = previous_terminal.sorting,
                            current_id = terminal.id
                        });
                }

                conn.Close();
                conn.Dispose();

                return true;
            }
        }
        public static bool Move_Down(BOL.Terminal.objTerminal terminal)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "* " +
                                "FROM " +
                                        "terminal " +
                                "WHERE " +
                                        "sorting > @sorting " +
                                "ORDER BY " +
                                        "sorting ASC " +
                                "LIMIT 1";

                BOL.Terminal.objTerminal previous_terminal = conn.Query<BOL.Terminal.objTerminal>(statement,
                    new
                    {
                        sorting = terminal.sorting
                    }).SingleOrDefault();

                if (previous_terminal != null)
                {
                    statement = "UPDATE " +
                                        "terminal " +
                                "SET " +
                                        "sorting = @current_sorting " +
                                "WHERE " +
                                        "id = @previous_id";
                    statement += ";UPDATE " +
                                        "terminal " +
                                "SET " +
                                        "sorting = @previous_sorting " +
                                "WHERE " +
                                        "id = @current_id";

                    conn.Execute(statement,
                        new
                        {
                            current_sorting = terminal.sorting,
                            previous_id = previous_terminal.id,
                            previous_sorting = previous_terminal.sorting,
                            current_id = terminal.id
                        });
                }

                conn.Close();
                conn.Dispose();

                return true;
            }
        }

    }
}
