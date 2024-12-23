using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;
using static GameConstants;

public class CatController : BaseCharacter
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(WAVE_TAG))
        {
            EventsManager.Instance.Notify(EventID.OnDecreaseCat);
        }
    }
}
