using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("Nivel 1");
        AudioManager.Instance.TransitionTo(AudioManager.Instance._ambienceMusic);
        AudioManager.Instance._snapshot.TransitionTo(2f);
    }

    public void MainMenu()
    { 
        SceneManager.LoadScene("Start Screen"); 
    }
}
