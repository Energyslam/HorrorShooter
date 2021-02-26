using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LerpAlphaUI : MonoBehaviour
{
    private TextMeshProUGUI text;
    [SerializeField]private float fadeTime = 1f;
    [SerializeField] private float visibleTime = 2f;
    private void Start()
    {
        text = this.GetComponent<TextMeshProUGUI>();
    }

    public void ShowTextField(int amount)
    {
        if (amount < 0)
        {
            text.text = "-" + amount;
        }
        else
        {
            text.text = "+" + amount;
        }
        StartCoroutine(e_ShowTextField());
    }

    IEnumerator e_ShowTextField()
    {
        Show();
        yield return new WaitForSeconds(visibleTime);
        Hide();
    }
    private void Show()
    {
        text.CrossFadeAlpha(1f, fadeTime, false);
    }

    private void Hide()
    {
        text.CrossFadeAlpha(0f, fadeTime, false);
    }
}
