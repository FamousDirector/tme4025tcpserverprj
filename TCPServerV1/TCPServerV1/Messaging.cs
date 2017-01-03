using System;
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

            return newcontrollerdata;
        }
        static public void StoreControllerData(DataClasses.ControllerData controllerdata)
        {
            DatabaseCalls.AddNewControllerDataToDatabase(controllerdata);
        }
        static public string CreateReplyMessage(DataClasses.ControllerData controllerdata)
        {
            string replymessage = "";

            DateTime time = DateTime.UtcNow.AddDays(1);
            long unixtimeinseconds = new DateTimeOffset(time).ToUnixTimeSeconds();
            replymessage = "<time=" + unixtimeinseconds.ToString() + ">";

            return replymessage;
        }
    }
}