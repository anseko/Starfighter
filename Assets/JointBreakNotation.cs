using UnityEngine;

public class JointBreakNotation : MonoBehaviour
{
    private void OnJointBreak(float breakForce)
    {
        Debug.unityLogger.Log($"JOINT BREAKS!!! {breakForce}");
    }
}
