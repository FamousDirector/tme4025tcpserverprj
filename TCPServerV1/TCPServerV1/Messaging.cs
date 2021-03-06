﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPServerV1
{
    static class Messaging
    {
        static public DataClasses.ControllerData ParseMessageData(string message)
        {
            //ex data : <UID = JAMESESP8266>< Relay = 0 >< Temperature = 85 >< Power = 88 >< EOM >
            DataClasses.ControllerData newcontrollerdata = new DataClasses.ControllerData();

            try
            { 
                char[] delimiterChars = { '<', '>',};
                string[] rawdatastrings = message.Split(delimiterChars);

                //Set UID
                string rawuid = Array.FindLast(rawdatastrings, s => s.StartsWith("UID"));
                newcontrollerdata.UID = rawuid.Remove(0, rawuid.LastIndexOf('=') + 1);

                //Set RelayState
                string rawrelaystate = Array.FindLast(rawdatastrings, s => s.StartsWith("Relay"));
                newcontrollerdata.RelayState = int.Parse(rawrelaystate.Remove(0, rawrelaystate.LastIndexOf('=') + 1));

                //Set Temperature
                string rawtemperature = Array.FindLast(rawdatastrings, s => s.StartsWith("Temperature"));
                newcontrollerdata.Temperature = int.Parse(rawtemperature.Remove(0, rawtemperature.LastIndexOf('=') + 1));

                //Set Power Usage
                string rawpower = Array.FindLast(rawdatastrings, s => s.StartsWith("Power"));
                newcontrollerdata.Power = int.Parse(rawpower.Remove(0, rawpower.LastIndexOf('=') + 1));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return newcontrollerdata;
        }
        static public void StoreControllerData(DataClasses.ControllerData controllerdata)
        {
            DatabaseCalls.AddNewControllerDataToDatabase(controllerdata);
        }
        static public string CreateReplyMessage(DataClasses.ControllerData controllerdata)
        {
            string replymessage = "";

            DateTime ontime = DatabaseCalls.GetNextRelayOnTime(controllerdata.UID);
            long ontime_unix = new DateTimeOffset(ontime).ToUnixTimeSeconds();
            replymessage += "<TimeOn=" + ontime_unix.ToString() + ">";

            DateTime offtime = DatabaseCalls.GetNextRelayOffTime(controllerdata.UID);
            long offtime_unix = new DateTimeOffset(offtime).ToUnixTimeSeconds();
            replymessage += "<TimeOff=" + offtime_unix.ToString() + ">";
            
            string newstate = DatabaseCalls.GetNewRelayState(controllerdata.UID);
            if (newstate != "") //ensure content
            {
                replymessage += "<RelayState=" + newstate + ">";
            }
            else
            {
                Console.WriteLine("No New State");
            }

            return replymessage;
        }
    }
}