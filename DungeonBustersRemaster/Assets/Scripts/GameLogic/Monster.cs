using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Monster
{
    public int DataId;
    public string DataName;
    public string Name;
    public int Dungeon;
    public int HP;
    public List<int> Reward1;
    public List<int> Reward2;
    public List<int> Reward3;
}

[Serializable]
public class MonsterList
{
    public List<Monster> monsters;
}