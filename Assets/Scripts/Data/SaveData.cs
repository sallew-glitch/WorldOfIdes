using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public PlayerData playerData;
    public WorldData worldData;    

    public SaveData()
    {
        playerData = new PlayerData();
        worldData = new WorldData();
    }
}

[System.Serializable]
public class PlayerData
{
    public float playerHP;
    public Vector3 position;
    public int level;
    public int experience;
    public int XPToLevelUp;
    public float maxHealth;
    public float playerMP;
    public float maxMP;
    public float playerStamina;
    public float maxStamina;
    public List<Item> inventory;
    public string sceneName;

    public PlayerData()
    {
        inventory = new List<Item>();
    }
}

[System.Serializable]
public class WorldData
{
    public Dictionary<string, bool> completedQuests;
    public List<NPCState> npcs;
    public List<EnemyState> enemies;

    public WorldData()
    {
        completedQuests = new Dictionary<string, bool>();
        npcs = new List<NPCState>();
        enemies = new List<EnemyState>();
    }
}

[System.Serializable]
public class NPCState
{
    public string npcID;
    public bool isAlive;
    public Vector3 position;
}

[System.Serializable]
public class EnemyState
{
    public string enemyID;
    public Vector3 position;
    public bool isDefeated;
}

