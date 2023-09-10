using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableLock : MonoBehaviour
{
    public void ShotLock()
    {
        Destroy(gameObject);
    }
}
