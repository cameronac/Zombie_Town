using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadShot : MonoBehaviour, IDamage
{
    [SerializeField] GameObject zombie;

    public void TakeDamage(float damage)
    {
        enemyAI ai = zombie.GetComponent<enemyAI>();
        IDamage d_comp = zombie.GetComponent<IDamage>();

        if (ai != null && d_comp != null) {
            d_comp.TakeDamage(damage);
            ai.wasHeadShot = true;
        }
    }
}
