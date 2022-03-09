namespace Core
{

    public enum UserType
    {   
        //Расположены в порядке возрастания прав (использование этого смотри в MainServerLoop.OnConnectCallback)
        Navigator,
        Spectator,
        SpaceStation,
        Pilot,
        Mechanic,
        Admin
    }

    public enum UnitState
    {
        InFlight,
        IsDocked,
        IsDead,
    }

    public enum ZoneType
    {
        RedZone,
        YellowZone,
        GreenZone,
        VioletZone,
        LightBlueZone,
        HealZone
    }

    public enum SpaceFieldType
    {
        SpaceField_Test,
        SpaceField_1,
        SpaceField_2,
        SpaceField_3
    }

}
