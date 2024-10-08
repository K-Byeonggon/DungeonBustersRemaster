using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MonsterDataManager : Singleton<MonsterDataManager>
{
    public Dictionary<int, Monster> LoadedMonsters = new Dictionary<int, Monster>();

    private Dictionary<string, GameObject> monsterPrefabs = new Dictionary<string, GameObject>();

    #region JSON Data

    public async UniTask LoadMonsterData()
    {
        // Resources 폴더에서 TextAsset으로 JSON 파일을 로드
        TextAsset jsonFile = Resources.Load<TextAsset>("Monsters");

        if (jsonFile != null)
        {
            // TextAsset의 텍스트 데이터를 읽어와서 JSON으로 변환
            string jsonData = jsonFile.text;
            MonsterList monsterList = JsonConvert.DeserializeObject<MonsterList>(jsonData);

            if (monsterList == null || monsterList.monsters == null)
            {
                Debug.LogError("몬스터 데이터를 불러오는데 실패했습니다. JSON 형식을 확인하세요.");
                return;
            }

            LoadedMonsters.Clear();
            foreach (Monster monster in monsterList.monsters)
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

    //던전 시작시, 해당 던전에 출현할 몬스터들의 Id를 구한다.
    public List<int> GetRandomMonsterDataIdsByDungeon(int currentDungeon, int numberOfMonsters = 4)
    {
        // 1. 특정 던전에 속하는 몬스터의 DataId만 필터링
        List<int> filteredMonsterIds = LoadedMonsters
            .Where(pair => pair.Value.Dungeon == currentDungeon) // Dungeon 정보가 일치하는지 확인
            .Select(pair => pair.Key)  // Monster의 DataId (Key)만 추출
            .ToList();

        // 1-1. 필터링된 몬스터가 없는 경우 예외 처리
        if (filteredMonsterIds.Count == 0)
        {
            Debug.LogWarning($"Dungeon {currentDungeon}에 해당하는 몬스터가 없습니다.");
            return new List<int>(); // 빈 리스트 반환
        }

        // 2. 무작위로 선택 가능한 수가 부족할 때 처리
        if (filteredMonsterIds.Count < numberOfMonsters)
        {
            Debug.LogWarning($"Dungeon {currentDungeon}에는 {numberOfMonsters}마리의 몬스터를 뽑을 수 없습니다.");
            return new List<int>(); // 빈 리스트 반환
        }

        // 3. 무작위로 몬스터 DataId 선택
        List<int> randomMonsterIds = filteredMonsterIds
            .OrderBy(x => Random.Range(0, filteredMonsterIds.Count)) // 무작위로 섞기
            .Take(numberOfMonsters) // 가능한 수만큼 선택
            .ToList();

        return randomMonsterIds;
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

            // Reward가 null일 경우를 처리해야 함
            if (monster.Reward == null)
            {
                Debug.LogWarning($"몬스터 ID: {monster.DataId}의 Reward 데이터가 없습니다.");
                continue;
            }

            // 이중 리스트 Reward 출력
            for (int i = 0; i < monster.Reward.Count; i++)
            {
                if (monster.Reward[i] != null)
                {
                    Debug.Log($"Reward{i + 1}: {string.Join(", ", monster.Reward[i])}");
                }
                else
                {
                    Debug.LogWarning($"몬스터 ID: {monster.DataId}의 Reward{i + 1} 데이터가 null입니다.");
                }
            }
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
                //prefab.name은 DataName과 동일해야함.
                monsterPrefabs[prefab.name] = prefab;
            }
        }
        await UniTask.Yield();
    }
    
    public GameObject GetMonsterPrefabByName(string dataName)
    {
        if (monsterPrefabs.ContainsKey(dataName))
        {
            return monsterPrefabs[dataName];
        }
        else
        {
            Debug.LogError($"Invalid DataName(prefad.Name): {dataName}");
            return null;
        }
    }

    public GameObject GetMonsterPrefabById(int dataId)
    {
        if (!LoadedMonsters.ContainsKey(dataId))
        {
            Debug.LogError($"Monster with DataId {dataId} not found in LoadedMonsters.");
            return null; // 적절한 기본값 반환 또는 예외 처리
        }

        string dataName = LoadedMonsters[dataId].DataName;

        GameObject monsterPrefab = GetMonsterPrefabByName(dataName);

        return monsterPrefab;
    }
    
    #endregion
}
