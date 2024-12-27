using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneController : MonoBehaviour
{
    [SerializeField] PlayableDirector _cutSceneIntro, _cutScenePhase2;
    [SerializeField] float _delay;
    [SerializeField] List<CinemachineVirtualCamera> _listStaticCams;

    private void Start()
    {
        EventsManager.Subscribe(GameEnums.EventID.OnStartPhase2, PlayCutScenePhase2);
        StartCoroutine(PlayCutscene());
    }

    private void OnDestroy()
    {
        EventsManager.Unsubscribe(GameEnums.EventID.OnStartPhase2, PlayCutScenePhase2);
    }

    private IEnumerator PlayCutscene()
    {
        yield return new WaitForSeconds(_delay);

        Debug.Log("play");
        _cutSceneIntro.Play();
        foreach (var cam in _listStaticCams)
            cam.gameObject.SetActive(true);
    }

    private void PlayCutScenePhase2(object obj) => _cutScenePhase2.Play();
}
