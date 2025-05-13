using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }    

    public GameObject _playerPrefab;

    public bool PAUSED = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);

        Time.timeScale = 0;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void Update()
    {
        if (InputManager.Instance)
            InputManager.Instance._inputController.InputControl();
        if (PlayerManager.Instance)
            PlayerManager.Instance._playerController.PlayerControl();       
    }
        
    public void ResumeGame()
    {        
        PAUSED = false;
    }
    public void PauseGame()
    {
        PAUSED = true;
    }

    public void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        List<string> sceneNames = new List<string>{ "City01", "Dungeon", "Graveyard", "Castle" };

        if (sceneNames.Contains(scene.name))
        {
            Time.timeScale = 1;
            GameObject spawnPoint = FindFirstObjectByType<SpawnPoint>().gameObject;
            _playerPrefab.transform.position = spawnPoint.transform.position;
            PlayerManager.Instance._spawnPoint = spawnPoint;
        }
    }

}
