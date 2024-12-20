using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _player;

    // Update is called once per frame
    void LateUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        transform.position = new Vector3(_player.position.x - 14.0f, transform.position.y, _player.position.z);
    }
}
