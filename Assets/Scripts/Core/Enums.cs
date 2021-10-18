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

    public enum UnitType
    {
        Ship,
        Asteroid,
        WayPoint,
        WorldObject
    }
    
    public enum UnitState
    {
        InFlight,
        IsDocked,
        IsDead,
    }

    public enum GameState
    {
        InMenu,
        InGame,
        OnExit //?
    }

    public enum InterpolationType
    {
        NoInterpolation,
        Linear,
        Square
    }
}
