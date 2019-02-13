using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{

    public GameSettings _settings; // drag GameSettings asset here in inspector
    [SerializeField]
    public static GameSettings s;
    public static Settings instance;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Settings.instance == null)
        {
            Settings.instance = this;
        }
        if (Settings.s == null)
        {
            Settings.s = _settings;
        }
    }
}
