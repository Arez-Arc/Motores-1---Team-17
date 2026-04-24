using UnityEngine;

public class ShipPart : MonoBehaviour
{
    AudioSource _audioSource;
    AudioClip _audioClip;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioClip = GetComponent<AudioClip>();
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
