using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMining
{
    public static class DBConn
    {
        public async static Task<int> GetCustomerCount()
        {
            return await Task.Run(() =>
            {

                northwndEntities northwindEntities = new northwndEntities();
                string nativeSQLQuery = "SELECT count(*) FROM Customers";
                var queryResult = northwindEntities.Database.SqlQuery<int>(nativeSQLQuery);
                try{
                int customersCount = queryResult.FirstOrDefault();
                }
                catch (Exception e)
                {
                    while (e.InnerException != null) e = e.InnerException;
                }
                Console.WriteLine(queryResult);

                return 0;
            });

        }
    }
}
