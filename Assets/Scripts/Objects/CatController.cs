using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;
using static GameConstants;

public struct CatInfor
{
    public int Index;
    public float PositionX;

    public CatInfor(int index, float posX)
    {
        Index = index;
        PositionX = posX;
    }
}

public class CatController : BaseCharacter
{
    private void Start()
    {
        EventsManager.Notify(EventID.OnCatSendPosition, transform.position.x);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(WAVE_TAG))
        {
            //EventsManager.Notify(EventID.OnDecreaseCat);
        }
    }
}
