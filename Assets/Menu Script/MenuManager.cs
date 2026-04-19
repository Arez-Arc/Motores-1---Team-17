using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("Nivel 1");
    }
}
