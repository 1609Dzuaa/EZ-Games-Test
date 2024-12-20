using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseSingleton<GameManager>
{
    [SerializeField] int _targetFrameRate;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = _targetFrameRate;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
