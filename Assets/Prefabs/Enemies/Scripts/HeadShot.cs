using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadShot : MonoBehaviour, IDamage
{
    [SerializeField] GameObject zombie;
    [SerializeField] GameObject head_mesh;

    public void TakeDamage(float damage)
    {
        enemyAI ai = zombie.GetComponent<enemyAI>();
        IDamage d_comp = zombie.GetComponent<IDamage>();

        if (ai != null && d_comp != null) {

            if (ai.currentHP - (damage * 2) <= 0)
            {
                head_mesh.SetActive(false);
            }

            d_comp.TakeDamage(damage * 2);
        }
    }
}
