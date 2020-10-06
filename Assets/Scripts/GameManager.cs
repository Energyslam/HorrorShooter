using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Singleton class for holding references to permanent objects such as the player, and performing global tasks.
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField] private GameObject _player;
    public GameObject Player { get { return _player; } }
    // Start is called before the first frame update
    public Canvas canvas;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI flareText;

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
    }

    public void UpdatePlayerHealth(int currentHealth)
    {
        //Is this dumb?
        playerHealthText.text = "" + currentHealth;
    }
}
