using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeHit : MonoBehaviour
{
    [SerializeField] playerShoot player;

    public void KnifeAttack()
    {
        if (player != null) {
            player.KnifeAttack();
        }
    }
}
