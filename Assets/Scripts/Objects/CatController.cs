using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

public class CatController : BaseCharacter
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wave"))
        {
            EventsManager.Instance.Notify(EventID.OnDecreaseCat);
        }
    }
}
