using UnityEngine;
using UnityEngine.SceneManagement;


[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

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
    }

    private void Update()
    {
        if (InputManager.Instance)
            InputManager.Instance._inputController.InputControl();
        if (PlayerManager.Instance)
            PlayerManager.Instance._playerController.PlayerControl();       
    }
        
    public void PauseGame()
    {
        PAUSED = true;
        SceneManager.LoadScene(0);
    }


}
