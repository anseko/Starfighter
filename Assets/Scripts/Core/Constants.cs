namespace Core
{
    public class Constants
    {
        public const string PathToPrefabs = "Prefabs/";
        public const string PathToAxes = "ScriptableObjects/Axes/";
        public const string PathToKeys = "ScriptableObjects/Keys/";
        public const string PathToAccounts = "ScriptableObjects/Accounts/";
        public const string PathToShipsObjects = "ScriptableObjects/Ships/";
        public const string PathToUnitsObjects = "ScriptableObjects/Units/";
        public const string PathToAsteroids = "./asteroids.json";
        public const string PathToShips = "./ships.dat";
        public const string PathToUnits = "./units.dat";

        //объекты, передаваемые в пакетах State от сервера, объекты контролируемые сервером
        public const string DynamicTag = "Dynamic";
        public const string AsteroidTag = "Asteroid";
        public const string WayPointTag = "WayPoint";
        public const string DockTag = "Dock";
        public const char Separator = '|';
        //Настройки камеры
        public const int ZoomStep = 1000;
        //Настройки навигатора

        public const int MaxPossibleDamageHp = 98;
       
    }
}