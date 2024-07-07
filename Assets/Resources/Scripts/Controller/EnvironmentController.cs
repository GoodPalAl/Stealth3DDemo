using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    // Unity Objects
    Transform player;
    //public Transform hiddingArea;

    [SerializeField]
    bool isHidingArea = true;

    //*
    private void Start()
    {
        player = PlayerManager.instance.player.transform;
    }
    //*/

    // Update is called once per frame
    void Update()
    {
        //*
        if (isHidingArea)
            Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>());
        //*/
    }
}
