namespace MurkysRustBoot.Classes
{

    public class UserEvent
    {
        string _name;
        int _interval;
        string _command;
        string _eventType;

        public string EventType
        {
            get { return _eventType; }
            set { _eventType = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }
        
        public string Command
        {
            get { return _command; }
            set { _command = value; }
        }
    }
}
