using Core;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ClientAccountObject", menuName = "Configs/ClientAccountObject", order = 0)]
    public class ClientAccountObject : ScriptableObject
    {
        public string login;
        public string password;
        public UserType type;
        public SpaceShipConfig ship;
        public ulong? clientId;
    }
}