namespace MurkysRustBoot.Classes
{
    public class Player
    {
        string _displayName;
        ulong _userID;
        string _ipAdress;
        

        public string IpAdress
        {
            get { return _ipAdress; }
            set { _ipAdress = value; }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public ulong UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }
    }
}
