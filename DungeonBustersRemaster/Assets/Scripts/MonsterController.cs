using Mirror.Examples.CCU;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterAnimation
{
    Idle,
    Attack,
    Damaged,
    Die,
    Win
}


public class MonsterController : MonoBehaviour
{
    /// <summary>
    /// key: DataId, Value: MonsterModelPrefab
    /// </summary>
    private Dictionary<int, GameObject> monsterModels = new Dictionary<int, GameObject>();

    private Animator animator;
    

    private void OnEnable()
    {
        SetMonster();
    }

    private void OnDisable()
    {
        ResetMonster();
    }


    //MonsterSpawner를 통해 활성화되면 실행됨.
    public void SetMonster()
    {
        //GameLogicManager에서 현재 몬스터의 DataId 가져오기
        int monsterDataId = GameLogicManager.Instance.CurrentMonsterDataId;

        //모델이 이미 있는지 Dictionary검사
        if (monsterModels.ContainsKey(monsterDataId))
        {
            monsterModels[monsterDataId].SetActive(true);
        }
        else
        {
            GameObject monsterModelPrefab = MonsterDataManager.Instance.GetMonsterPrefabById(monsterDataId);
            GameObject monsterModel = Instantiate(monsterModelPrefab, transform);

            monsterModels[monsterDataId] = monsterModel;
        }

        animator = monsterModels[monsterDataId].GetComponent<Animator>();
    }

    public void ResetMonster()
    {
        int monsterDataId = GameLogicManager.Instance.CurrentMonsterDataId;

        if (monsterModels.ContainsKey(monsterDataId))
        {
            monsterModels[monsterDataId].SetActive(false);
        }

        animator = null;
    }

    public void SetAnimator(MonsterAnimation anim)
    {

    }
}
