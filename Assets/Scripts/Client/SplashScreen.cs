using Core;
using Net.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    
public class SplashScreen : MonoBehaviour
{
    [SerializeField]private int _maxNumAsteroids;
    private float _currentNumAsteroids;
    private float _loadRatio;
    [SerializeField] private GameObject _screen;
    private void Awake()
    {
        _loadRatio = Mathf.RoundToInt(_currentNumAsteroids / _maxNumAsteroids * 100);
    }

    // Start is called before the first frame update
    private void LoadScene(int num)
    {
        _maxNumAsteroids = num;
        _currentNumAsteroids = GameObject.FindGameObjectsWithTag(Constants.AsteroidTag).Length;
        while(_loadRatio < 90)
        {
            GetComponent<Slider>().value = _loadRatio;
        }
        _screen.SetActive(false);
    }
}
}