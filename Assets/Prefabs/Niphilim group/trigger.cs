using UnityEngine;

public class trigger : MonoBehaviour
{
    public Animator animator;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.unityLogger.Log("sTART");
            animator.playableGraph.Play();
        }
    }
}
