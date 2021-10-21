namespace Core
{

    public enum UserType
    {   
        //Расположены в порядке возрастания прав (использование этого смотри в MainServerLoop.OnConnectCallback)
        Navigator,
        Spectator,
        Pilot,
        Moderator, //игротех
        Admin
    }

    public enum UnitState
    {
        InFlight,
        IsDocked,
        IsDead,
    }

}
