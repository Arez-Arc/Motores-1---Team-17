using UnityEngine;

public class ShipPart : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollectPart();
            _audioSource.PlayOneShot(_audioClip);
            gameObject.SetActive(false);
        }
    }
}
