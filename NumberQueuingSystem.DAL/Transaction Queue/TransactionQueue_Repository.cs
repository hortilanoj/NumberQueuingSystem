using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace NumberQueuingSystem.DAL.Transaction_Queue
{
    public static class TransactionQueue_Repository
    {
        public static BOL.Transaction_Queue.objTransactionQueue AddNew(BOL.Transaction.objTransaction trans)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT IFNULL(MAX(queue_number), 0) + 1 FROM transaction_queue WHERE transaction_id = @transaction_id AND DATE(date_time) = CURRENT_DATE() ";

                int next_queue = conn.Query<int>(statement, 
                    new 
                    { 
                        transaction_id = trans.id 
                    }).SingleOrDefault();

                statement = "INSERT INTO " +
                                        "transaction_queue " +
                                    "(transaction_id, queue_number, date_time) " +
                                "VALUES " +
                                    "(@transaction_id, @queue_number, CURRENT_TIMESTAMP())";

                statement += ";SELECT " +
                                    "TQ.*, T.* " +
                             "FROM " +
                                    "transaction_queue TQ " +
                                        "INNER JOIN transaction T ON TQ.transaction_id = T.id " +
                             "ORDER BY " +
                                    "TQ.id DESC " +
                             "LIMIT 1";

                BOL.Transaction_Queue.objTransactionQueue TransQueue = conn.Query<BOL.Transaction_Queue.objTransactionQueue, BOL.Transaction.objTransaction, BOL.Transaction_Queue.objTransactionQueue>
                    (statement,
                    (TQ, T) => { TQ.objTransaction = T; return TQ; },
                    new 
                    { 
                        transaction_id = trans.id, 
                        queue_number = next_queue 
                    }).SingleOrDefault();

                conn.Close();

                return TransQueue;
            }
        }

        public static bool TransferQueue(BOL.Terminal.objTerminal target_terminal, BOL.Terminal_Queue.objTerminalQueue queue)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "INSERT INTO " +
                                        "transaction_queue " +
                                    "(transaction_id, terminal_id, queue_number, date_time) " +
                                "VALUES " +
                                    "(@transaction_id, @target_terminal, @queue_number, CURRENT_TIMESTAMP())";

                statement += ";UPDATE " +
                                    "terminal_queue " +
                             "SET " +
                                    "is_done = @is_done " +
                             "WHERE " +
                                    "id = @id";

                conn.Execute
                    (statement,
                    new
                    {
                        id = queue.id,
                        is_done = true,
                        transaction_id = queue.transaction_id,
                        target_terminal = target_terminal.id,
                        queue_number = queue.objTransactionQueue.queue_number
                    });

                conn.Close();
                conn.Dispose();

                return true;
            }
        }

        public static bool DeleteAllTransactionQueue()
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "DELETE FROM " +
                                    "transaction_queue " +
                                "WHERE " +
                                    "id IS NOT NULL";

                conn.Execute(statement);

                conn.Close();
                conn.Dispose();

                return true;
            }
        }

        public static List<BOL.Transaction_Queue.objTransactionQueue> GetWaitingList(BOL.Terminal.objTerminal terminal)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "SELECT " +
                                      "TQ.*, T.* " +
                              "FROM " +
                                      "transaction_queue TQ " +
                                          "INNER JOIN transaction T ON TQ.transaction_id = T.id " +
                              "WHERE " +
                                      "TQ.terminal_id = @terminal_id AND TQ.id NOT IN (SELECT transaction_queue_id FROM terminal_queue) AND DATE(TQ.date_time) = CURRENT_DATE() " +
                              "ORDER BY " +
                                      "TQ.id ASC";

                IEnumerable<BOL.Transaction_Queue.objTransactionQueue> ForwardedQueue = conn.Query<BOL.Transaction_Queue.objTransactionQueue, BOL.Transaction.objTransaction, BOL.Transaction_Queue.objTransactionQueue>(statement,
                    (TQ, T) => { TQ.objTransaction = T; return TQ; },
                    new
                    {
                        terminal_id = terminal.id
                    }).ToList();

                if (ForwardedQueue == null)
                    ForwardedQueue = new List<BOL.Transaction_Queue.objTransactionQueue>();

                statement = "SELECT " +
                                        "TQ.*, T.* " +
                                "FROM " +
                                        "transaction_queue TQ " +
                                            "INNER JOIN transaction T ON TQ.transaction_id = T.id " +
                                                "INNER JOIN terminal_transaction TT ON T.id = TT.transaction_id " +
                                "WHERE " +
                                        "TQ.id NOT IN (SELECT transaction_queue_id FROM terminal_queue) AND TT.terminal_id = @terminal_id AND TQ.terminal_id IS NULL AND DATE(TQ.date_time) = CURRENT_DATE() " +
                                "ORDER BY " +
                                        "TT.priority_level ASC, TQ.queue_number ASC, TQ.id ASC";

                IEnumerable<BOL.Transaction_Queue.objTransactionQueue> TransQueue = conn.Query<BOL.Transaction_Queue.objTransactionQueue, BOL.Transaction.objTransaction, BOL.Transaction_Queue.objTransactionQueue>(statement,
                    (TQ, T) => { TQ.objTransaction = T; return TQ; },
                    new
                    {
                        terminal_id = terminal.id
                    }).ToList();

                if (TransQueue == null)
                    TransQueue = new List<BOL.Transaction_Queue.objTransactionQueue>();

                IEnumerable<BOL.Transaction_Queue.objTransactionQueue> WaitingList = ForwardedQueue.Union(TransQueue);

                conn.Close();
                conn.Dispose();

                return WaitingList.ToList();
            }
        }

        public static bool ReturnQueue(BOL.Terminal_Queue.objTerminalQueue queue)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "SELECT " +
                                        "TRQ.*, TNQ.* " +
                                "FROM " +
                                        "terminal_queue TRQ " +
                                            "INNER JOIN transaction_queue TNQ ON TRQ.transaction_queue_id = TNQ.id " +
                                "WHERE " +
                                        "TNQ.queue_number = @queue_number AND DATE(TNQ.date_time) = CURRENT_DATE() AND TRQ.terminal_id != @terminal_id AND TRQ.is_done = @is_done " +
                                "ORDER BY " +
                                        "TRQ.id DESC, TNQ.id DESC " +
                                "LIMIT 1";

                BOL.Terminal_Queue.objTerminalQueue Previous = conn.Query<BOL.Terminal_Queue.objTerminalQueue, BOL.Transaction_Queue.objTransactionQueue, BOL.Terminal_Queue.objTerminalQueue>(statement,
                   (TRQ, TNQ) => { TRQ.objTransactionQueue = TNQ; return TRQ; },
                   new
                   {
                       is_done = true,
                       terminal_id = queue.terminal_id,
                       queue_number = queue.objTransactionQueue.queue_number
                   }).SingleOrDefault();

                if (Previous != null)
                {
                    statement = "INSERT INTO " +
                                        "transaction_queue " +
                                    "(transaction_id, terminal_id, queue_number, date_time) " +
                                "VALUES " +
                                    "(@transaction_id, @target_terminal, @queue_number, CURRENT_TIMESTAMP())";

                    statement += ";UPDATE " +
                                        "terminal_queue " +
                                 "SET " +
                                        "is_done = @is_done " +
                                 "WHERE " +
                                        "id = @id";

                    conn.Execute
                        (statement,
                        new
                        {
                            id = queue.id,
                            is_done = true,
                            transaction_id = queue.transaction_id,
                            target_terminal = Previous.terminal_id,
                            queue_number = queue.objTransactionQueue.queue_number
                        });

                    conn.Close();
                    conn.Dispose();

                    return true;

                }

                conn.Close();
                conn.Dispose();

                return false;
            }
        }
    }
}
