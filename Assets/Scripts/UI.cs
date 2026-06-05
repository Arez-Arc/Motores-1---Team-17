using UnityEngine;
using TMPro;
using System.Collections;

public class UI : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    public float displayDuration = 3f;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        ShowMessage("Find all the ship parts to escape");
    }

    void OnEnable()
    {
        GameManager.OnPartCollected += UpdatePartMessage;
    }

    void OnDisable()
    {
       
        GameManager.OnPartCollected -= UpdatePartMessage;
    }

    void UpdatePartMessage(int current, int total)
    {
        int remaining = total - current;
        if (remaining > 1)
            ShowMessage($"Part found! {remaining} parts remaining.");
        else if (remaining == 1)
        {
            ShowMessage("Only 1 part left, HURRY");
        }else if (remaining < 1)
        {
            ShowMessage("All the spiders know were you are \n RUN TO THE SPACESHIP");
        }
            
    }

    public void ShowMessage(string message)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayRoutine(message));
    }

    IEnumerator DisplayRoutine(string message)
    {
        textMesh.text = message;
        textMesh.enabled = true;

        yield return new WaitForSeconds(displayDuration);

        textMesh.enabled = false;
    }
}
