using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace NumberQueuingSystem.DAL.Terminal_Queue
{
    public static class TerminalQueue_Repository
    {

        public static bool Next(BOL.Terminal.objTerminal terminal, BOL.Terminal_Queue.objTerminalQueue terminal_queue)
        {
          
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "UPDATE " +
                                       "terminal_queue " +
                                   "SET " +
                                           "is_done = @is_done " +
                                   "WHERE " +
                                           "id = @id";

                conn.Execute(statement,
                    new
                    {
                        id = terminal_queue.id,
                        is_done = true
                    });

                statement = "SELECT " +
                                       "TQ.*, T.* " +
                               "FROM " +
                                       "transaction_queue TQ " +
                                           "INNER JOIN transaction T ON TQ.transaction_id = T.id " +
                               "WHERE " +
                                       "TQ.terminal_id = @terminal_id AND TQ.id NOT IN (SELECT transaction_queue_id FROM terminal_queue) AND DATE(TQ.date_time) = CURRENT_DATE() " +
                               "ORDER BY " +
                                       "TQ.id ASC " +
                               "LIMIT 1";

                BOL.Transaction_Queue.objTransactionQueue ForwardedQueue = conn.Query<BOL.Transaction_Queue.objTransactionQueue, BOL.Transaction.objTransaction, BOL.Transaction_Queue.objTransactionQueue>(statement,
                    (TQ, T) => { TQ.objTransaction = T; return TQ; },
                    new
                    {
                        terminal_id = terminal.id
                    }).SingleOrDefault();

                if (ForwardedQueue == null)
                {
                    statement = "SELECT " +
                                       "TQ.*, T.* " +
                               "FROM " +
                                       "transaction_queue TQ " +
                                           "INNER JOIN transaction T ON TQ.transaction_id = T.id " +
                                               "INNER JOIN terminal_transaction TT ON T.id = TT.transaction_id " +
                               "WHERE " +
                                       "TQ.id NOT IN (SELECT transaction_queue_id FROM terminal_queue) AND TT.terminal_id = @terminal_id AND TQ.terminal_id IS NULL AND DATE(TQ.date_time) = CURRENT_DATE() " +
                               "ORDER BY " +
                                       "TT.priority_level ASC, TQ.queue_number ASC, TQ.id ASC " +
                               "LIMIT 1";

                    BOL.Transaction_Queue.objTransactionQueue TransQueue = conn.Query<BOL.Transaction_Queue.objTransactionQueue, BOL.Transaction.objTransaction, BOL.Transaction_Queue.objTransactionQueue>(statement,
                        (TQ, T) => { TQ.objTransaction = T; return TQ; },
                        new
                        {
                            terminal_id = terminal.id
                        }).SingleOrDefault();

                    if (TransQueue == null)
                    {
                        return false;
                    }

                    statement = "INSERT INTO " +
                                    "terminal_queue " +
                                "(terminal_id, transaction_queue_id, is_done) " +
                            "VALUES " +
                                "(@terminal_id, @transaction_queue_id, @is_done)";

                    conn.Execute(statement,
                        new
                        {
                            transaction_id = TransQueue.objTransaction.id,
                            terminal_id = terminal.id,
                            transaction_queue_id = TransQueue.id,
                            is_done = false
                        });

                    conn.Close();
                    conn.Dispose();

                    return true;
                }
                
                statement = "INSERT INTO " +
                                    "terminal_queue " +
                                "(terminal_id, transaction_queue_id, is_done) " +
                            "VALUES " +
                                "(@terminal_id, @transaction_queue_id, @is_done)";

                conn.Execute(statement,
                    new
                    {
                        transaction_id = ForwardedQueue.objTransaction.id,
                        terminal_id = terminal.id,
                        transaction_queue_id = ForwardedQueue.id,
                        is_done = false
                    });

                conn.Close();
                conn.Dispose();

                return true;
            }
        }

        public static BOL.Terminal_Queue.objTerminalQueue GetCurrentQueue(BOL.Terminal.objTerminal terminal)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "TRQ.*, TNQ.*,  T.* " +
                                "FROM " +
                                        "terminal_queue TRQ " +
                                            "INNER JOIN transaction_queue TNQ ON TRQ.transaction_queue_id = TNQ.id " +
                                                "INNER JOIN transaction T ON TNQ.transaction_id = T.id " +
                                "WHERE " +
                                        "TRQ.terminal_id = @terminal_id AND TRQ.is_done = @is_done AND DATE(TNQ.date_time) = CURRENT_DATE() " +
                                "ORDER BY " +
                                        "TRQ.id DESC " +
                                "LIMIT 1";

                BOL.Terminal_Queue.objTerminalQueue Queue = conn.Query<BOL.Terminal_Queue.objTerminalQueue, BOL.Transaction_Queue.objTransactionQueue, BOL.Transaction.objTransaction, BOL.Terminal_Queue.objTerminalQueue>(statement,
                    (TRQ, TNQ, T) => { TRQ.objTransactionQueue = TNQ; TNQ.objTransaction = T ; return TRQ; },
                    new
                    {
                        is_done = false,
                        terminal_id = terminal.id,
                    }).SingleOrDefault();

                if (Queue == null)
                    Queue = new BOL.Terminal_Queue.objTerminalQueue();

                conn.Close();
                conn.Dispose();

                return Queue;
            }        
        }

        public static List<BOL.Terminal_Queue.objTerminalQueue> GetTerminalQueueHistory(BOL.Terminal.objTerminal terminal)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "TRQ.*, TNQ.*, T.* " +
                                "FROM " +
                                        "terminal_queue TRQ " +
                                            "INNER JOIN transaction_queue TNQ ON TRQ.transaction_queue_id = TNQ.id " +
                                                "INNER JOIN transaction T ON TNQ.transaction_id = T.id " +
                                "WHERE " +
                                        "TRQ.terminal_id = @terminal_id AND DATE(TNQ.date_time) = CURRENT_DATE() AND TRQ.is_done = @is_done " +
                                "ORDER BY " +
                                        "TRQ.id DESC";

                List<BOL.Terminal_Queue.objTerminalQueue> Queue = conn.Query<BOL.Terminal_Queue.objTerminalQueue, BOL.Transaction_Queue.objTransactionQueue, BOL.Transaction.objTransaction, BOL.Terminal_Queue.objTerminalQueue>(statement,
                    (TRQ, TNQ, T) => { TRQ.objTransactionQueue = TNQ; TNQ.objTransaction = T; return TRQ; },
                    new
                    {
                        is_done = true,
                        terminal_id = terminal.id,
                    }).ToList();

                conn.Close();
                conn.Dispose();

                return Queue;
            }
        }

        public static bool Set_History_As_Current(BOL.Terminal_Queue.objTerminalQueue terminal_queue)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "UPDATE " +
                                        "terminal_queue " +
                                "SET " +
                                        "is_done = @previous_done " +
                                "WHERE " +
                                        "terminal_id = @terminal_id";

                statement += ";INSERT INTO " +
                                        "terminal_queue " +
                                    "(terminal_id, transaction_queue_id, is_done) " +
                                "VALUES " +
                                    "(@terminal_id, @transaction_queue_id, @is_done)";

                conn.Execute(statement,
                    new
                    {
                        previous_done = true,
                        transaction_id = terminal_queue.transaction_id,
                        terminal_id = terminal_queue.terminal_id,
                        transaction_queue_id = terminal_queue.transaction_queue_id,
                        is_done = false
                    });

                conn.Close();
                conn.Dispose();

                return true;
            }
        }

        public static bool Set_Waiting_As_Current(BOL.Transaction_Queue.objTransactionQueue transaction_queue, BOL.Terminal.objTerminal terminal)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "UPDATE " +
                                        "terminal_queue " +
                                "SET " +
                                        "is_done = @previous_done " +
                                "WHERE " +
                                        "terminal_id = @terminal_id";

                statement += ";INSERT INTO " +
                                        "terminal_queue " +
                                    "(terminal_id, transaction_queue_id, is_done) " +
                                "VALUES " +
                                    "(@terminal_id, @transaction_queue_id, @is_done)";

                conn.Execute(statement,
                    new
                    {
                        previous_done = true,
                        transaction_id = transaction_queue.objTransaction.id,
                        terminal_id = terminal.id,
                        transaction_queue_id = transaction_queue.id,
                        is_done = false
                    });

                conn.Close();
                conn.Dispose();

                return true;
            }
        }

    }
}
