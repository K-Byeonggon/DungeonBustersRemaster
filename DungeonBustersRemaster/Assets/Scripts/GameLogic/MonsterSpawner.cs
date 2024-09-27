using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterSpawner : MonoBehaviour
{
    public static Transform monsterPosition;

    private GameObject monster;

    private void Start()
    {
        //GameLogicManager.Instance.OnMonsterSpawn += SpawnMonster;
    }


    //MonsterPosition을 가진 오브젝트에서 OnEnable에 실행함.
    public static void RegisterMonsterPosition(Transform position)
    {
        monsterPosition = position;
    }

    public static void UnregisterMonsterPosition()
    {
        monsterPosition = null;
    }

    //아마 StageStart에서 몬스터를 스폰하려고 하면 불러짐.
    public void SpawnMonster(int monsterDataId)
    {
        //유효한 몬스터 번호인지 검사
        if(!MonsterDataManager.Instance.LoadedMonsters.ContainsKey(monsterDataId))
        {
            Debug.Log($"Invalid DataId: {monsterDataId}");
            return;
        }

        //스폰 장소 검사
        if(monsterPosition == null)
        {
            Debug.LogError("monsterPosition Unregistered.");
            return;
        }

        //몬스터 스폰
        if(monster != null)
        {
            MonsterController monsterCtrl = monster.GetComponent<MonsterController>();
            monsterCtrl.ResetMonster();

            monsterCtrl.SetMonster(monsterDataId);
            monster.SetActive(true);
        }
        else
        {
            GameObject monsterPrefab = MonsterDataManager.Instance.GetMonsterPrefabByName("Monster");
            if (monsterPrefab != null)
            {
                monster = Instantiate(monsterPrefab, monsterPosition.position, monsterPosition.rotation);
                MonsterController monsterCtrl = monster.GetComponent<MonsterController>();
                
                monsterCtrl.SetMonster(monsterDataId);
                monster.SetActive(true);
            }
            else
            {
                Debug.LogError("monsterPrefab is null");
                return;
            }
        }
    }

}
