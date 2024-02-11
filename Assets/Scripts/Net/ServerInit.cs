using System;
using System.IO;
using Core;
using Mirror;
using Net.Core;
using TMPro;
using UnityEngine;

namespace Net
{
    [RequireComponent(typeof(ServerInitializeHelper))]
    public class ServerInit: MonoBehaviour
    {
        public TextMeshProUGUI clientCounter;
        public void Awake()
        {
            NetEventStorage.GetInstance().WorldInit.AddListener(_ => 
                StarfighterNetworkManager.singleton.StartServer());
        }

        public  void Start()
        {
            try
            {
                var spacefield = File.ReadAllText(Constants.PathToAsteroids);
                var field = Resources.Load<GameObject>(Constants.PathToPrefabs + spacefield);
                var fieldGO = Instantiate(field, Vector3.zero, new Quaternion(0, 180, 0, 1));
                StartCoroutine(GetComponent<ServerInitializeHelper>().InitServer());
            }
            catch (FileNotFoundException notFoundException)
            {
                var spacefield = File.ReadAllText(Constants.PathToAsteroids + "Spacefield_Test");
                var field = Resources.Load<GameObject>(Constants.PathToPrefabs + spacefield);
                var fieldGO = Instantiate(field, Vector3.zero, Quaternion.identity);
                StartCoroutine(GetComponent<ServerInitializeHelper>().InitServer());
            }
        }

        private void Update()
        {
            
            clientCounter.text = NetworkManager.singleton.numPlayers.ToString();
        }
    }
}