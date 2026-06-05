using UnityEngine;

public class WinGame : MonoBehaviour
{
 
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.OnShipExit();
        }
    }
}
