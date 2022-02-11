using System.Collections;
using System.Security.Cryptography;
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
                unitScript.currentHp.Value = unitScript.unitConfig.maxHp;
            }

            yield return new WaitForSecondsRealtime(_dayInterval);
        }
    }
    
    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }
}
