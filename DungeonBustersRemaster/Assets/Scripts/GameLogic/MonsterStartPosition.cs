using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStartPosition : MonoBehaviour
{
    private void OnEnable()
    {
        MonsterSpawner.RegisterMonsterPosition(transform);
    }

    private void OnDisable()
    {
        MonsterSpawner.UnregisterMonsterPosition();
    }
}
