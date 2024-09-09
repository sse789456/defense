using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousePartCollisionController : MonoBehaviour
{
    [NonSerialized] public HousePart housePart;

    private void OnCollisionEnter(Collision other)
    {
        housePart.OnCollisionEnter_Custom(other);
    }
}
