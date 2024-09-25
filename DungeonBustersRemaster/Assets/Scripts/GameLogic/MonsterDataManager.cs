using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MonsterDataManager : Singleton<MonsterDataManager>
{
    public string jsonFilePath = "Assets/Resources/Monsters.json";
    public Dictionary<int, Monster> LoadedMonsters = new Dictionary<int, Monster>();

    private Dictionary<string, GameObject> monsterPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<int, GameObject> monsterPool = new Dictionary<int, GameObject>();

    #region JSON Data

    public async UniTask LoadMonsterData()
    {
        if (File.Exists(jsonFilePath))
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            MonsterList monsterList = JsonUtility.FromJson<MonsterList>(jsonData);

            if (monsterList == null || monsterList.monsters == null)
            {
                Debug.LogError("몬스터 데이터를 불러오는데 실패했습니다. JSON 형식을 확인하세요.");
                return;
            }

            LoadedMonsters.Clear();
            foreach(Monster monster in monsterList.monsters)
            {
                LoadedMonsters[monster.DataId] = monster;
            }

            Debug.Log("몬스터 데이터를 성공적으로 불러왔습니다.");
        }
        else
        {
            Debug.LogError("Json파일을 찾을 수 없습니다.");
        }
        await UniTask.Yield();
    }

    //디버그용. LoadMonsterData 이후에 부르던가 하면 됨.
    public void VerifyLoadedMonsters()
    {
        if (LoadedMonsters == null || LoadedMonsters.Count == 0)
        {
            Debug.LogError("LoadedMonsters 데이터가 비어있습니다!");
            return;
        }

        Debug.Log($"총 {LoadedMonsters.Count}개의 몬스터 데이터를 불러왔습니다.");

        // LoadedMonsters에 있는 각 몬스터 데이터를 출력
        foreach (KeyValuePair<int, Monster> entry in LoadedMonsters)
        {
            Monster monster = entry.Value;
            Debug.Log($"몬스터 ID: {monster.DataId}, 이름: {monster.Name}, 던전: {monster.Dungeon}, HP: {monster.HP}");

            // 리워드 출력
            string reward1 = string.Join(", ", monster.Reward1);
            string reward2 = string.Join(", ", monster.Reward2);
            string reward3 = string.Join(", ", monster.Reward3);

            Debug.Log($"Reward1: {reward1}, Reward2: {reward2}, Reward3: {reward3}");
        }
    }
    #endregion

    #region Monster Prefab

    public async UniTask LoadAllMonsterPrefabs()
    {
        var prefabs = Resources.LoadAll<GameObject>("Prefabs/Monster");
        foreach(var prefab in prefabs)
        {
            if (prefab != null)
            {
                //prefab.name은 DataId와 동일해야함.
                monsterPrefabs[prefab.name] = prefab;
            }
        }
        await UniTask.Yield();
    }

    
    public GameObject GetMonsterPrefab(string dataId)
    {
        if (monsterPrefabs.ContainsKey(dataId))
        {
            return monsterPrefabs[dataId];
        }
        else
        {
            Debug.LogError($"Invalid DataId(prefad.Name): {dataId}");
            return null;
        }
    }
    
    #endregion
}
