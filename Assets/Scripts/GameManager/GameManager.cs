using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameEnums;

public class GameManager : BaseSingleton<GameManager>
{
    [SerializeField] int _targetFrameRate;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = _targetFrameRate;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
    }

    public void ReloadScene()
    {
        //SceneManager.sceneLoaded += ReloadObject;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
