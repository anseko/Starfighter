using System.Collections;
using System.Collections.Generic;
using Client.Core;
using UnityEngine;

public class SearchPlayers : MonoBehaviour
{
    [SerializeField] private GameObject _beacon;
    private Queue<PlayerScript> _ships;
    
    // Start is called before the first frame update
    public void ShowPlayers()
    {
        var players = FindObjectsOfType<PlayerScript>();
        _ships = new Queue<PlayerScript>();
        foreach (var x in players)
        {
            if (x.GetComponent<Renderer>().enabled)
            {
                _ships.Enqueue(x);
            }
        }

        foreach (var x in _ships)
        {
            var _thisBeacon = Instantiate(_beacon,x.transform);
            _thisBeacon.gameObject.SetActive(true);
            Destroy(_thisBeacon,3);
        }
    }
}
