using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneController : MonoBehaviour
{
    [SerializeField] PlayableDirector _cutSceneClip;
    [SerializeField] float _delay;
    [SerializeField] List<CinemachineVirtualCamera> _listStaticCams;

    private void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        yield return new WaitForSeconds(_delay);

        Debug.Log("play");
        _cutSceneClip.Play();
        foreach (var cam in _listStaticCams)
            cam.gameObject.SetActive(true);
    }
}
