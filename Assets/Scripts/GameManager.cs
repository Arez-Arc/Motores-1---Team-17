using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Victoria")]
    [SerializeField] private int _totalShipParts = 4;
    private int _partsCollected;

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

    void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Death Screen");
        AudioManager.Instance?.TransitionTo(AudioManager.Instance._mainTheme);
    }

    public void CollectPart()
    {
        _partsCollected++;
        if(_partsCollected == _totalShipParts)
        {

            WinGame();
            _partsCollected = 0;
        }
    }

    public void RestartGame()
    {
        _partsCollected = 0;
        SceneManager.LoadScene("Nivel 1");
    }
}
