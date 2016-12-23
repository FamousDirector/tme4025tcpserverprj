namespace TCPServerV1
{
    static public class DataClasses
    {
        public class ControllerData
        {
            public string UID { get; set; }
            public int RelayState { get; set; }
            public int Temperature { get; set; }
            public int Power { get; set; }
        }
    }
}
