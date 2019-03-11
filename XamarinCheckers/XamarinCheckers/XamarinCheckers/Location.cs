namespace XamarinCheckers
{
    public class Location
    {
        public int xCoord;
        public int yCoord;

        public Location(int x, int y)
        {
            this.xCoord = x;
            this.yCoord = y;
        }

        public Location()
        {
        }

        public static bool operator == (Location lhs, Location rhs)
        {
            if (lhs.xCoord == rhs.xCoord && lhs.yCoord == rhs.yCoord)
                return true;
            else
                return false;
        }

        public bool Equals (Location rhs)
        {
            return this == rhs;
        }

        public static bool operator != (Location lhs, Location rhs)
        {
            if (lhs.xCoord != rhs.xCoord || lhs.yCoord != rhs.yCoord)
                return true;
            else
                return false;
        }
    }
}