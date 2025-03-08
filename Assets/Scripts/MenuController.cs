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
        //Temporary Logic
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }

    public void OpenSettings()
    {

    }

    public void QuitGame()
    {

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
