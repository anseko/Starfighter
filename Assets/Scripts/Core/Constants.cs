namespace Core
{
    public class Constants
    {
        public const string PathToPrefabs = "Prefabs/";
        public const string PathToShipsPrefabs = "Prefabs/Ships/";
        public const string PathToAxes = "ScriptableObjects/Axes/";
        public const string PathToKeys = "ScriptableObjects/Keys/";
        public const string PathToAccounts = "ScriptableObjects/Accounts/";
        public const string PathToShipsObjects = "ScriptableObjects/Ships/";
        public const string PathToUnitsObjects = "ScriptableObjects/Units/";
        public const string PathToDangerZones = "ScriptableObjects/DangerZones/";
        public const string PathToAsteroids = "./asteroids.dat";
        public const string PathToShips = "./ships.dat";
        public const string PathToUnits = "./units.dat";

        //объекты, передаваемые в пакетах State от сервера, объекты контролируемые сервером
        public const string DynamicTag = "Dynamic";
        public const string AsteroidTag = "Asteroid";
        public const string WayPointTag = "WayPoint";
        public const string OrderFrameTag = "StaticFrames";
        public const string AIPointTag = "AIPoint";
        public const string DockTag = "Dock";
        public const char Separator = '|';
        //Настройки камеры
        public const int ZoomStep = 1000;
        //Настройки навигатора
       
        //Прочие настройки
        public const int MaxPossibleDamageHp = 93;
        

    }
}