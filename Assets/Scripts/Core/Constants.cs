﻿namespace Core
{
    public class Constants
    {
        public const float WorldUpdateTimeout = 0.001f;
        
        public const string MulticastAddress = "239.255.255.250";
        
        public const int ServerReceivingPort = 40001;
        public const int ServerSendingPort = 40000;

        public const string PathToPrefabs = "Prefabs/";
        public const string PathToAxes = "ScriptableObjects/Axes/";
        public const string PathToKeys = "ScriptableObjects/Keys/";
        public const string PathToAccounts = "ScriptableObjects/Accounts/";
        public const string PathToShipsObjects = "ScriptableObjects/Ships/";
        public const string PathToAsteroids = "./asteroids.json";
        public const string PathToShips = "./ships.dat";

        //объекты, передаваемые в пакетах State от сервера, объекты контролируемые сервером
        public const string DynamicTag = "Dynamic";
        public const string AsteroidTag = "Asteroid";
        public const string WayPointTag = "WayPoint";
        public const char Separator = '|';
        //Настройки камеры
        public const int ZoomStep = 1000;
        //Настройки навигатора
       
    }
}