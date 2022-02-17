using UnityEngine;

namespace Core
{
    public class FramesLimiter : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.antiAliasing = 4;
        }

        // Update is called once per frame
        private void Update()
        {
        
        }
    }
}
