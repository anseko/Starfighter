﻿namespace Net.Core
{
    public class Constants
    {
        public const string MulticastAddress = "239.255.255.250";
        
        public const int ServerReceivingPort = 5001;
        public const int ServerSendingPort = 5000;

        public const string PathToPrefabs = "Prefabs/";
        
        //объекты, передаваемые в пакетах State от сервера, объекты контролируемые сервером
        public const string DynamicTag = "Dynamic";
        //объекты. передаваемые в пакетах State от клиента, объекты контролируемые клиентом (должны ли быть такие вообще?)
        public const string PlayerTag = "Player"; 
    }
}