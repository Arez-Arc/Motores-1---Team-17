using System.Collections;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // Instancia estática para acceder desde cualquier lado
    public static AudioManager Instance;

    [Header("Configuración de Audio")]
    public AudioClip _mainTheme;
    public AudioClip _ambienceMusic;
    public AudioClip _chaseMusic;
    public AudioClip _deathSound;
    public AudioMixerSnapshot _snapshot;

    [Header("Referencias de Sources")]
    public AudioSource _sourceA;
    public AudioSource _sourceB;

    private AudioSource currentSource;
    private AudioSource nextSource;

    private int chasingCount = 0;
    private AudioClip actualClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentSource = _sourceA;
        nextSource = _sourceB;

        actualClip = _mainTheme;
        currentSource.clip = actualClip;
        currentSource.volume = 1;
        currentSource.Play();
    }

    public void TransitionTo(AudioClip newClip, float duration = 1.5f)
    {
        if (newClip == actualClip) return;
        actualClip = newClip;

        nextSource.clip = newClip;

        StopAllCoroutines();
        StartCoroutine(CrossfadeCoroutine(duration));
    }
    public void UpdateChaseStatus(bool startingChase)
    {
        if (startingChase) chasingCount++;
        else chasingCount--;

        chasingCount = Mathf.Max(0, chasingCount);

        
        if (chasingCount > 0)
        {
            TransitionTo(_chaseMusic);
        }
        else
        {
            TransitionTo(_ambienceMusic);
        }
    }

    public void PlayDeathSound()
    {
        AudioSource _player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<AudioSource>();

        _player.PlayOneShot(_deathSound);
    
        return;
    }

    IEnumerator CrossfadeCoroutine(float duration)
    {
        nextSource.Play();
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / duration;

            nextSource.volume = Mathf.Lerp(0, 1, t);
            currentSource.volume = Mathf.Lerp(1, 0, t);
            yield return null;
        }

        currentSource.Stop();

        AudioSource temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;
    }
}