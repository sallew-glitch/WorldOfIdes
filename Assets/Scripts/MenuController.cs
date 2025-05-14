using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Threading.Tasks;


[RequireComponent(typeof(BoxCollider))]
public class MenuController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject _pauseMenu;
    public void PlayGame()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void ContinueGame()
    {                
        SaveManager.Instance.LoadGame();
    }

    public void Levels()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 5);
    }

    public void OpenSettings()
    {

    }

    public async void SaveGame()
    {
        ResumeGame();
        await Task.Yield();
        SaveManager.Instance.SaveGame();
    }

    public async void LoadGame()
    {
        ResumeGame();
        await Task.Yield();
        SaveManager.Instance.LoadGame();
    }

    public void ResumeGame()
    {
        GameManager.Instance.PAUSED = false;              

        _pauseMenu.SetActive(false);
    }

    public void PauseGame()
    {
        GameManager.Instance.PAUSED = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _pauseMenu.SetActive(true);
    }

    public void Back()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse Over Option");
        Transform line = transform.Find("Line");
        if (line != null)
        {
            line.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Transform line = transform.Find("Line");
        if (line != null)
        {
            line.gameObject.SetActive(false);
        }
    }
}
