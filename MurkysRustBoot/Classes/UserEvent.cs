namespace MurkysRustBoot.Classes
{

    public class UserEvent
    {
        /// <param name="name">Custom user event name.</param>
        /// <param name="interval">Interval of the event in seconds.</param>
        /// <param name="command">The command sent to the server.</param>
        /// <param name="eventType">The type of the event.</param>
        /// <param name="isActivate">If the event should run or not.</param>
        public UserEvent(string name, int interval, string command, string eventType)
        {
            Name = name;
            Interval = interval;
            Command = command;
            EventType = eventType;
        }

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
