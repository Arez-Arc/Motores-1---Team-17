using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private bool _isDead = false;

    public void TakeDamage()
    {
        if (_isDead) return;
        _isDead = true;
        Die();
    }

    private void Die()
    {
        GetComponent<PlayerController>().enabled = false;

        AudioManager.Instance?.PlayDeathSound();

        Invoke("DeadScene", 5f);

    }

    private void DeadScene()
    {
        SceneManager.LoadScene("Death Screen");
    }
}
