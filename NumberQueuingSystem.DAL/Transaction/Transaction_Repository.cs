using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace NumberQueuingSystem.DAL.Transaction
{
    public static class Transaction_Repository
    {
        public static List<BOL.Transaction.objTransaction> GetAll()
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "* " +
                                "FROM " +
                                        "transaction";

                List<BOL.Transaction.objTransaction> Transactions = conn.Query<BOL.Transaction.objTransaction>(statement).ToList();

                conn.Close();
                conn.Dispose();

                return Transactions;
            }       
        }

        public static List<BOL.Transaction.objTransaction> GetActiveTransaction()
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {
                var statement = "SELECT " +
                                        "* " +
                                "FROM " +
                                        "transaction " +
                                "WHERE " +
                                        "active = @active";

                List<BOL.Transaction.objTransaction> Transactions = conn.Query<BOL.Transaction.objTransaction>(statement, new { active = true }).ToList();

                conn.Close();
                conn.Dispose();

                return Transactions;
            }       
        }

        public static List<BOL.Transaction.objTransaction> GetAvailableTransactions(BOL.Terminal.objTerminal terminal)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "SELECT " +
                                        "* " +
                                "FROM " +
                                        "transaction " +
                                "WHERE " +
                                        "active = @active AND id NOT IN (SELECT transaction_id FROM terminal_transaction WHERE terminal_id = @terminal_id)";


                List<BOL.Transaction.objTransaction> List = conn.Query<BOL.Transaction.objTransaction>(statement,
                    new
                    {
                        active= true,
                        terminal_id = terminal.id
                    }).ToList();
                conn.Close();
                conn.Dispose();

                return List;
            }
        }

        public static bool Add(BOL.Transaction.objTransaction transaction)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "INSERT INTO " +
                                        "transaction " +
                                "(name, prefix, description, active) " +
                                "VALUES " +
                                "(@name, @prefix, @description, @active)";

                conn.Execute(statement,
                    new
                    {
                        name = transaction.name.Trim(),
                        prefix = transaction.prefix.Trim(),
                        description = transaction.description.Trim(),
                        active = transaction.active
                    });


                conn.Close();
                conn.Dispose();

                return true;
            }      
        }

        public static bool Update(BOL.Transaction.objTransaction transaction)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "UPDATE " +
                                        "transaction " +
                                "SET " +
                                        "name = @name, " +
                                        "prefix = @prefix, " +
                                        "description = @description, " +
                                        "active = @active " +
                                "WHERE " +
                                        "id = @id";

                conn.Execute(statement,
                    new
                    {
                        id = transaction.id,
                        name = transaction.name.Trim(),
                        prefix = transaction.prefix.Trim(),
                        description = transaction.description.Trim(),
                        active = transaction.active
                    });


                conn.Close();
                conn.Dispose();

                return true;
            }
        }

        public static bool Delete(BOL.Transaction.objTransaction transaction)
        {
            using (var conn = DAL.Base.ConnectionManager.Connection)
            {

                var statement = "DELETE FROM " +
                                        "transaction " +
                                "WHERE " +
                                        "id = @id";
                conn.Execute(statement,
                    new
                    {
                        id = transaction.id
                    });


                conn.Close();
                conn.Dispose();

                return true;
            }      
        }
    }
}
