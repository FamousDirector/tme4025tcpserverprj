using System;
using System.Configuration;
using System.Data.SqlClient;

namespace TCPServerV1
{
    public class DatabaseCalls
    {
        internal static void AddNewControllerDataToDatabase(DataClasses.ControllerData controllerdata)
        {
            using (SqlConnection myConnection = getDatabaseConnection())
            {
                using (SqlCommand myCommand = new SqlCommand())
                {
                    string cmdString = "INSERT INTO ControllerDataTable " +
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
#if DEBUG
            //store entries in HourlyBilling DB           
            string databaseName = "HybernateDatabase"; 
            string server = "JAMESADCAMERON\\SQLEXPRESS";
            
            //create connectionString 
            string connectionString =  "server =" + server + "; " +
                                       "Trusted_Connection=sspi;" + //uses the applications users credientials                                      
                                       "database=" + databaseName + "; " +
                                       "connection timeout=30";
#else
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
#endif
            Console.WriteLine("DB Connection String : " + connectionString); //debug

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

        internal static string GetNewRelayState(string UID)
        {
            string newstate = "";

            //get new state From DB 
            //if exists: get state that has most recent date and remove row all rows for that UID 
            //else return empty

            using (SqlConnection myConnection = getDatabaseConnection())
            {
                using (SqlCommand myCommand = new SqlCommand())
                {
                    string cmdString = "SELECT * FROM NewRelayState WHERE UID = @uid";
                    myCommand.Connection = myConnection;
                    myCommand.CommandText = cmdString;
                    myCommand.Parameters.AddWithValue("@uid", UID);
                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        try
                        {
                            if (myReader.Read())
                            {
                                int value = int.Parse(myReader["NewState"].ToString());
                                if (value == 0)
                                {
                                    newstate = "OFF";
                                }
                                else
                                {
                                    newstate = "ON";
                                }
                            }
                        }

                        catch (SqlException error)
                        {
                            Console.WriteLine("Error occured when getting new relay state from the DB: " + error.Message);
                        }
                    }
                }

                using (SqlCommand myCommand = new SqlCommand())
                {
                    string cmdString = "DELETE FROM NewRelayState WHERE UID = @uid";
                    myCommand.Connection = myConnection;
                    myCommand.CommandText = cmdString;
                    myCommand.Parameters.AddWithValue("@uid", UID);

                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (SqlException error)
                    {
                        Console.WriteLine("Error occured when removing new relay state from the DB: " + error.Message);
                    }
                }
            }
            return newstate;            
        }

        internal static DateTime GetNextRelayOnTime(string UID)
        {
            int ontime = getRelayOnTime(UID);
            DateTime nextontime = DateTime.UtcNow.Date.AddSeconds(ontime);

            return nextontime;
        }

        internal static DateTime GetNextRelayOffTime(string UID)
        {
            int offtime = getRelayOffTime(UID);
            DateTime nextofftime = DateTime.UtcNow.Date.AddSeconds(offtime);

            return nextofftime;
        }

        private static int getRelayOnTime(string UID)
        {
            int ontime = -999999999;
            using (SqlConnection myConnection = getDatabaseConnection())
            {
                using (SqlCommand myCommand = new SqlCommand())
                {
                    string cmdString = "SELECT ScheduledOnTimeSeconds FROM DeviceSchedules WHERE UID = @uid";
                    myCommand.Connection = myConnection;
                    myCommand.CommandText = cmdString;
                    myCommand.Parameters.AddWithValue("@uid", UID);

                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        try
                        {
                            if (myReader.Read())
                            {
                                ontime = int.Parse(myReader["ScheduledOnTimeSeconds"].ToString());
                            }
                        }

                        catch (SqlException error)
                        {
                            Console.WriteLine("Error occured when getting Scheduled On Time from the DB: " + error.Message);
                        }
                    }

                }
            }
            return ontime;
        }

        private static int getRelayOffTime(string UID)
        {
            int offtime = -999999999;
            using (SqlConnection myConnection = getDatabaseConnection())
            {
                using (SqlCommand myCommand = new SqlCommand())
                {
                    string cmdString = "SELECT ScheduledOffTimeSeconds FROM DeviceSchedules WHERE UID = @uid";
                    myCommand.Connection = myConnection;
                    myCommand.CommandText = cmdString;
                    myCommand.Parameters.AddWithValue("@uid", UID);

                    try
                    {
                        using (SqlDataReader myReader = myCommand.ExecuteReader())
                        {
                            if (myReader.Read())
                            {
                                offtime = int.Parse(myReader["ScheduledOffTimeSeconds"].ToString());
                            }
                        }
                    }
                    catch (SqlException error)
                    {
                        Console.WriteLine("Error occured when getting Scheduled Off Time from the DB: " + error.Message);
                    }
                }
            }
            return offtime;
        }
    }
}
