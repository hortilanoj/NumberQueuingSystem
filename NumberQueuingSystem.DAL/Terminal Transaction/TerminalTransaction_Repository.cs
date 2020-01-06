using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace NumberQueuingSystem.DAL.Terminal_Transaction
{
    public static class TerminalTransaction_Repository
    {
        public static List<BOL.Terminal_Transaction.objTerminalTransaction> GetTerminalTransactions(BOL.Terminal.objTerminal terminal)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "SELECT " +
                                        "TT.*, T.* " +
                                "FROM " +
                                        "terminal_transaction TT " +
                                            "INNER JOIN transaction T ON TT.transaction_id = T.id " +
                                "WHERE " +
                                        "TT.terminal_id = @terminal_id " +
                                "ORDER BY " +
                                        "TT.priority_level ASC";

                List<BOL.Terminal_Transaction.objTerminalTransaction> Transactions = conn.Query<BOL.Terminal_Transaction.objTerminalTransaction, BOL.Transaction.objTransaction, BOL.Terminal_Transaction.objTerminalTransaction>(statement,
                    (TT, T) => { TT.objTransaction = T; return TT; },
                    new
                    {
                        terminal_id = terminal.id
                    }).ToList();

                conn.Close();
                conn.Dispose();

                return Transactions;
            }
        }

        public static bool AddNew_Transaction(BOL.Terminal.objTerminal terminal, BOL.Transaction.objTransaction transaction)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "SELECT " +
                                        "IFNULL(MAX(priority_level), 0) + 1 " +
                                "FROM " +
                                        "terminal_transaction " +
                                "WHERE " +
                                        "terminal_id = @terminal_id";

                int next_level = conn.Query<int>(statement,
                                    new
                                    {
                                        terminal_id = terminal.id
                                    }).SingleOrDefault();

                statement = "INSERT INTO " +
                                         "terminal_transaction " +
                                     "(terminal_id, transaction_id, priority_level) " +
                                "VALUES " +
                                     "(@terminal_id, @transaction_id, @priority_level)";

                conn.Execute(statement,
                    new
                    {
                        terminal_id = terminal.id,
                        transaction_id = transaction.id,
                        priority_level = next_level
                    });

                conn.Close();
                conn.Dispose();

                return true;
            }
        }

        public static bool Remove(BOL.Terminal_Transaction.objTerminalTransaction terminal_transaction)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "UPDATE " +
                                        "terminal_transaction " +
                                "SET " +
                                        "priority_level = priority_level - 1 " +
                                "WHERE " +
                                       "terminal_id = @terminal_id AND priority_level > @priority_level";

                statement += ";DELETE FROM " +
                                        "terminal_transaction " +
                             "WHERE " +
                                        "id = @id";

                conn.Execute(statement,
                    new
                    {
                        terminal_id = terminal_transaction.terminal_id,
                        priority_level = terminal_transaction.priority_level,
                        id = terminal_transaction.id
                    });

                conn.Close();
                conn.Dispose();

                return true;
            }
        }

        public static bool Move_Up(BOL.Terminal_Transaction.objTerminalTransaction terminal_transaction)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "SELECT " +
                                       "TT.*, T.* " +
                               "FROM " +
                                       "terminal_transaction TT " +
                                           "INNER JOIN transaction T ON TT.transaction_id = T.id " +
                               "WHERE " +
                                       "TT.terminal_id = @terminal_id AND priority_level < @priority_level AND TT.id != @id " +
                               "ORDER BY " +
                                       "TT.priority_level DESC " +
                               "LIMIT 1";

                BOL.Terminal_Transaction.objTerminalTransaction previous_termninaltransaction = conn.Query<BOL.Terminal_Transaction.objTerminalTransaction, BOL.Transaction.objTransaction, BOL.Terminal_Transaction.objTerminalTransaction>(statement,
                    (TT, T) => { TT.objTransaction = T; return TT; },
                    new
                    {
                        terminal_id = terminal_transaction.terminal_id,
                        priority_level = terminal_transaction.priority_level,
                        id = terminal_transaction.id
                    }).SingleOrDefault();

                if (previous_termninaltransaction != null)
                {
                    statement = "UPDATE " +
                                        "terminal_transaction " +
                                "SET " +
                                        "priority_level = @priority_down " +
                                "WHERE " +
                                        "id = @down_id";
                    statement += ";UPDATE " +
                                        "terminal_transaction " +
                                "SET " +
                                        "priority_level = @priority_up " +
                                "WHERE " +
                                        "id = @up_id";


                    conn.Execute(statement,
                        new
                        {
                            priority_down = terminal_transaction.priority_level,
                            down_id = previous_termninaltransaction.id,
                            priority_up = previous_termninaltransaction.priority_level,
                            up_id = terminal_transaction.id
                        });
                }

                conn.Close();
                conn.Dispose();

                return true;
            }
        }

        public static bool Move_Down(BOL.Terminal_Transaction.objTerminalTransaction terminal_transaction)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "SELECT " +
                                       "TT.*, T.* " +
                               "FROM " +
                                       "terminal_transaction TT " +
                                           "INNER JOIN transaction T ON TT.transaction_id = T.id " +
                               "WHERE " +
                                       "TT.terminal_id = @terminal_id AND priority_level > @priority_level AND TT.id != @id " +
                               "ORDER BY " +
                                       "TT.priority_level ASC " +
                               "LIMIT 1";

                BOL.Terminal_Transaction.objTerminalTransaction previous_termninaltransaction = conn.Query<BOL.Terminal_Transaction.objTerminalTransaction, BOL.Transaction.objTransaction, BOL.Terminal_Transaction.objTerminalTransaction>(statement,
                    (TT, T) => { TT.objTransaction = T; return TT; },
                    new
                    {
                        terminal_id = terminal_transaction.terminal_id,
                        priority_level = terminal_transaction.priority_level,
                        id = terminal_transaction.id
                    }).SingleOrDefault();

                if (previous_termninaltransaction != null)
                {
                    statement = "UPDATE " +
                                        "terminal_transaction " +
                                "SET " +
                                        "priority_level = @priority_down " +
                                "WHERE " +
                                        "id = @previous_id";
                    statement += ";UPDATE " +
                                        "terminal_transaction " +
                                "SET " +
                                        "priority_level = @priority_up " +
                                "WHERE " +
                                        "id = @current_id";


                    conn.Execute(statement,
                        new
                        {
                            priority_down = terminal_transaction.priority_level,
                            previous_id = previous_termninaltransaction.id,
                            priority_up = previous_termninaltransaction.priority_level,
                            current_id = terminal_transaction.id
                        });
                }

                conn.Close();
                conn.Dispose();

                return true;
            }
        }
    }
}
