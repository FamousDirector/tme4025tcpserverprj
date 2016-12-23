using System;
using System.Data.SqlClient;

namespace TCPServerV1
{
    public class DatabaseCalls
    {
        public static void AddNewControllerDataToDatabase(DataClasses.ControllerData controllerdata)
        {
            using (SqlConnection myConnection = getDatabaseConnection())
            {
                using (SqlCommand myCommand = new SqlCommand())
                {
                    string cmdString = "INSERT INTO ControllerDataTable" +
                        "(UID,RelayState,Temperature,Power) VALUES (@uid, @relaystate, @temperature, @power)";
                    myCommand.Connection = myConnection;
                    myCommand.CommandText = cmdString;
                    myCommand.Parameters.AddWithValue("@uid", controllerdata.UID);
                    myCommand.Parameters.AddWithValue("@relaystate", controllerdata.RelayState);
                    myCommand.Parameters.AddWithValue("@temperature", controllerdata.Temperature);
                    myCommand.Parameters.AddWithValue("@power", controllerdata.Power);

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch(SqlException error)
                    {
                        Console.WriteLine("Error occured when adding new Controller Data to the DB: " + error.Message);
                    }
                }
            }
        }

        private static SqlConnection getDatabaseConnection()
        {
            //store entries in HourlyBilling DB           
            string databaseName = "HybernateDatabase"; 
            string server = "JAMESADCAMERON\\SQLEXPRESS";
            
            //create connectionString 
            string connectionString =  "server =" + server + "; " +
                                       "Trusted_Connection=sspi;" + //uses the applications users credientials                                      
                                       "database=" + databaseName + "; " +
                                       "connection timeout=30";

            SqlConnection myConnection = new SqlConnection(connectionString);
            try
            {
                myConnection.Open();
                return myConnection;
            }

            catch (Exception ex)
            {
                throw new Exception("Could not connect to the database: " + ex.Message);
            }
        }
    }
}
