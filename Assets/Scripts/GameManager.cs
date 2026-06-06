using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Victoria")]
    [SerializeField] private int _totalShipParts = 5;
    private int _partsCollected;
    private bool _canWin = false;

    public static bool EnemyAlarmActivated{ get; private set; } = false;
    public static event Action<int, int> OnPartCollected;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else
        {
            Destroy(gameObject);
        }
    }

    public void WinGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("End Screen");
        AudioManager.Instance?.TransitionTo(AudioManager.Instance._mainTheme);
    }

    public void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Death Screen");
        AudioManager.Instance?.TransitionTo(AudioManager.Instance._mainTheme);
    }

    public void CollectPart()
    {
        _partsCollected++;
        OnPartCollected?.Invoke(_partsCollected, _totalShipParts);

        if(_partsCollected == _totalShipParts)
        {
            Alert();
            _canWin = true;
        }
     
    }

    void Alert()
    {
        EnemyAlarmActivated = true;
        AudioManager.Instance?.PlayAlert();

        StartCoroutine(AlertEnemies(1.5f));
    }

    IEnumerator AlertEnemies(float delay)
    {
        yield return new WaitForSeconds(delay);

        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enem in allEnemies)
        {
            enem.ActivateAlert();
         
        } 
    }
    public void OnShipExit()
    {
        if (_partsCollected == _totalShipParts && _canWin)
        {
            WinGame();
            _partsCollected = 0;
            EnemyAlarmActivated = false;
        }
    }

    public void RestartGame()
    {
        _partsCollected = 0;
        SceneManager.LoadScene("Nivel 1");
    }
}
