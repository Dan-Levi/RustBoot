namespace MurkysRustBoot.Classes
{
    public class Player
    {
        string displayName;
        ulong userID;
        string ipAdress;
        

        public string IpAdress
        {
            get { return ipAdress; }
            set { ipAdress = value; }
        }

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        public ulong UserID
        {
            get { return userID; }
            set { userID = value; }
        }
    }
}
