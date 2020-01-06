using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace NumberQueuingSystem.DAL.Task
{
    public static class Task_Repository
    {
        public static BOL.Task.objTask GetDisplayCurrentTask()
        {

            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "* " +
                                "FROM " +
                                        "task " +
                                "WHERE " +
                                        "type != @refresh_frontdesk AND type != @refresh_clientterminal " +
                                "ORDER BY " +
                                        "id ASC " +
                                "LIMIT 1";

                BOL.Task.objTask Task = conn.Query<BOL.Task.objTask>(statement, 
                    new { 
                            refresh_frontdesk = BOL.Task.objTask.MessageType.RefreshFrontDesk,
                            refresh_clientterminal = BOL.Task.objTask.MessageType.RefreshClientTerminal
                        }).SingleOrDefault();

                if (Task != null)
                {
                    statement = "DELETE FROM " +
                                    "task " +
                                "WHERE " +
                                    "id = @id";

                    conn.Execute(statement, new { id = Task.id });
                }
                conn.Close();
                conn.Dispose();
                return Task;
            }
        }

        public static BOL.Task.objTask GetFrontdeskTask()
        {

            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "* " +
                                "FROM " +
                                        "task " +
                                "WHERE " +
                                        "type = @type " + 
                                "ORDER BY " +
                                        "id ASC " +
                                "LIMIT 1";

                BOL.Task.objTask Task = conn.Query<BOL.Task.objTask>(statement,
                    new
                    {
                        type = BOL.Task.objTask.MessageType.RefreshFrontDesk
                    }).SingleOrDefault();

                if (Task != null)
                {
                    statement = "DELETE FROM " +
                                    "task " +
                                "WHERE " +
                                    "id = @id";

                    conn.Execute(statement, new { id = Task.id });
                }

                conn.Close();
                conn.Dispose();
                return Task;
            }
        }

        public static BOL.Task.objTask GetTerminalClientTask(BOL.Terminal.objTerminal terminal)
        {

            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "* " +
                                "FROM " +
                                        "task " +
                                "WHERE " +
                                        "type = @type AND terminal_id = @terminal_id " +
                                "ORDER BY " +
                                        "id ASC " +
                                "LIMIT 1";

                BOL.Task.objTask Task = conn.Query<BOL.Task.objTask>(statement,
                    new
                    {
                        terminal_id = terminal.id,
                        type = BOL.Task.objTask.MessageType.RefreshClientTerminal
                    }).SingleOrDefault();

                if (Task != null)
                {
                    statement = "DELETE FROM " +
                                    "task " +
                                "WHERE " +
                                    "id = @id";

                    conn.Execute(statement, new { id = Task.id });
                }

                conn.Close();
                conn.Dispose();
                return Task;
            }
        }

        public static bool AddNew_Task(BOL.Task.objTask task)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "INSERT INTO " +
                                        "task " +
                                    "(type, terminal_id) " +
                                "VALUES " +
                                    "(@type, @terminal_id)";


                conn.Execute(statement,
                    new
                    {
                        type = task.type,
                        terminal_id = task.terminal_id
                    });

                conn.Close();
                conn.Dispose();
                return true;
            }
        }

        public static void Delete_Task(BOL.Task.objTask task)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "DELETE FROM " +
                                        "task " +
                                "WHERE " +
                                        "id = @id";

                    conn.Execute(statement,
                        new
                        {
                            id = task.id
                        });

                    conn.Close();
                    conn.Dispose();
            }
        }

        public static void Delete_All_Task()
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "DELETE FROM " +
                                        "task " +
                                "WHERE " +
                                        "id IS NOT NULL";

                conn.Execute(statement);

                conn.Close();
                conn.Dispose();
            }
        }

        public static void Add_Refresh_AllClientTerminal_Task()
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

                foreach(var terminal in List)
                {
                    statement = "INSERT INTO " +
                                        "task " +
                                    "(type, terminal_id) " +
                                "VALUES " +
                                    "(@type, @terminal_id)";


                    conn.Execute(statement,
                        new
                        {
                            type = BOL.Task.objTask.MessageType.RefreshClientTerminal,
                            terminal_id = terminal.id
                        });
                }

                conn.Close();
                conn.Dispose();
            }
        }
    }
}
