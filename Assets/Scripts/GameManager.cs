using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton class for holding references to permanent objects such as the player, and performing global tasks.
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField] private GameObject _player;
    public GameObject Player { get { return _player; } }
    public Canvas canvas;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI flareText;
    public TextMeshProUGUI ghoulKillsText;
    public Image bloodscreen;

    private int ghoulKills = 0;
    private int playerMaxHealth;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        else if (_instance != null)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerMaxHealth = Player.GetComponent<Player>().MaxHealth;
    }

    public void UpdateGhoulKills()
    {
        ghoulKills++;
        ghoulKillsText.text = ghoulKills.ToString();
    }

    public void UpdatePlayerHealth(int currentHealth)
    {
        playerHealthText.text = currentHealth.ToString();
        //alphaMultiplier = 1f - (float)currentHealth / 100f;
        //m_bloodscreen.SetFloat("_AlphaMultiplier", alphaMultiplier);
        Color tmp = bloodscreen.color;
        tmp.a = 1f - (float)currentHealth / playerMaxHealth;
        bloodscreen.color = tmp;
    }

    public void SwapToGameOverScene()
    {
        SceneManager.LoadScene("GameOver");
    }
}
