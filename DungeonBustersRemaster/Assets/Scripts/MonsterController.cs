using Mirror.Examples.CCU;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationState
{
    Idle,
    Running,
    Defeated,
    Attack,
    Damaged,
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
        //SetMonster();
    }

    private void OnDisable()
    {
        //ResetMonster();
    }


    //MonsterSpawner를 통해 활성화되면 실행됨.
    public void SetMonster(int monsterDataId)
    {

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
        foreach(var kvp in monsterModels)
        {
            if (kvp.Value.activeSelf)
            {
                kvp.Value.SetActive(false);
            }
        }

        animator = null;
    }

    public void SetAnimator(AnimationState anim)
    {
        switch (anim)
        {
            case AnimationState.Idle:
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsDefeated", false);
                break;
            case AnimationState.Running:
                animator.SetBool("IsRunning", true);
                break;
            case AnimationState.Defeated:
                animator.SetBool("IsDefeated", true);
                break;
            case AnimationState.Attack:
                animator.SetTrigger("Attack");
                break;
            case AnimationState.Damaged:
                animator.SetTrigger("Damaged");
                break;
            case AnimationState.Win:
                animator.SetTrigger("Win");
                break;
            default:
                Debug.LogError($"Invalid AnimationState {anim.ToString()}");
                break;
        }
    }
}
