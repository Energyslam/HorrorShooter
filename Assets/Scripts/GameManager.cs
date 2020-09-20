using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField] private GameObject _player;
    public GameObject Player { get { return _player; } }
    // Start is called before the first frame update

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
}
