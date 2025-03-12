using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider))]
public class MenuController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void PlayGame()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

    public void QuitGame()
    {

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
