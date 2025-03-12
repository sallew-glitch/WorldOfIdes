using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;
    public static SaveManager Instance { get { return _instance; } }

    private string savePath;
    private SaveData loadedData;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        savePath = Application.persistentDataPath + "/savefile.json";
        DontDestroyOnLoad(gameObject);
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        // Save Player Data
        data.playerData.playerHP = PlayerManager.Instance.playerHP;
        data.playerData.position = PlayerManager.Instance.transform.position;
        data.playerData.level = PlayerManager.Instance.playerLevel;
        data.playerData.experience = PlayerManager.Instance.playerXP;
        data.playerData.inventory = InventoryManager.Instance.items;
        data.playerData.maxHealth = PlayerManager.Instance.maxHP;
        data.playerData.playerMP = PlayerManager.Instance.playerMP;
        data.playerData.maxMP = PlayerManager.Instance.maxMP;
        data.playerData.playerStamina = PlayerManager.Instance.playerStamina;
        data.playerData.maxStamina = PlayerManager.Instance.maxStamina;
        data.playerData.sceneName = SceneManager.GetActiveScene().name;

        // Save World Data (if needed)
        // data.worldData.completedQuests = WorldManager.Instance.GetQuestStates();
        // data.worldData.npcs = WorldManager.Instance.GetNPCStates();
        // data.worldData.enemies = WorldManager.Instance.GetEnemyStates();

        // Convert to JSON and write to file
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game Saved to: " + savePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found!");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        if (data == null)
        {
            Debug.LogError("Failed to parse save data.");
            return;
        }

        // Store the loaded data for later use.
        loadedData = data;

        // Use SceneManager.sceneLoaded to apply the data after scene load.
        SceneManager.sceneLoaded += OnSceneLoaded;              
        SceneManager.LoadScene(loadedData.playerData.sceneName);

        // Load the scene that was saved (if you are also saving scene name)
        // For now, assume the game loads the current scene.
        // If you saved a scene name, use:
        // SceneManager.LoadScene(data.playerData.sceneName);
        // Otherwise, you can simply call OnSceneLoaded manually if staying in the same scene:
        // OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        // If you don't change scenes, you can call OnSceneLoaded immediately:
        // OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe so this only happens once
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Ensure the managers exist before applying loaded data.
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("PlayerManager instance is not found!");
            return;
        }
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager instance is not found!");
            return;
        }

        // Load Player Data
        PlayerManager.Instance.playerHP = loadedData.playerData.playerHP;
        PlayerManager.Instance.transform.position = loadedData.playerData.position;
        PlayerManager.Instance.playerLevel = loadedData.playerData.level;
        PlayerManager.Instance.playerXP = loadedData.playerData.experience;
        InventoryManager.Instance.items = loadedData.playerData.inventory;
        PlayerManager.Instance.maxHP = loadedData.playerData.maxHealth;
        PlayerManager.Instance.playerMP = loadedData.playerData.playerMP;
        PlayerManager.Instance.maxMP = loadedData.playerData.maxMP;
        PlayerManager.Instance.playerStamina = loadedData.playerData.playerStamina;
        PlayerManager.Instance.maxStamina = loadedData.playerData.maxStamina;

        Debug.Log("Game Loaded from: " + savePath);

        // Clear loadedData reference if no longer needed
        loadedData = null;
    }
}
