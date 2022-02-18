using System.Collections;
using Client.Core;
using UnityEngine;

public class Reseter : MonoBehaviour
{
    private const int _dayInterval = 8 * 3600;
    private Coroutine _timer;
    private void Awake()
    {
        _timer = StartCoroutine(OnElapsed());
    }

    private IEnumerator OnElapsed()
    {
        while (true)
        {
            foreach (var unitScript in FindObjectsOfType<PlayerScript>())
            {
                Debug.unityLogger.Log($"Timer elapsed:{unitScript.gameObject.name}");
                unitScript.NetworkUnitConfig.CurrentHp = unitScript.NetworkUnitConfig.MaxHp;
                unitScript.NetworkUnitConfig.CurrentStress = 0;
            }

            yield return new WaitForSecondsRealtime(_dayInterval);
        }
    }
    
    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }
}
