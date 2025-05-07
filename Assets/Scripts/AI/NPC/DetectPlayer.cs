using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    public bool playerDetected = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            playerDetected = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            playerDetected = false;
    }
}
