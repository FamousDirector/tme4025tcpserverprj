using System;
using System.Configuration;
using static TCPServerV1.TCPServer;

namespace TCPServerV1
{
    public class EntryPoint
    {
        public static int Main(string[] args)
        {
#if DEBUG
            //store entries in HourlyBilling DB           
            string databaseName = "HybernateDatabase";
            string server = "JAMESADCAMERON\\SQLEXPRESS";

            //create connectionString 
            string connectionString = "server =" + server + "; " +
                                       "Trusted_Connection=sspi;" + //uses the applications users credientials                                      
                                       "database=" + databaseName + "; " +
                                       "connection timeout=30";
#else
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
#endif
            Console.WriteLine("DB Connection String : " + connectionString); //debug
            AsynchronousSocketListener.StartListening();
            return 0;
        }
    }
}

