using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScrollViewController : MonoBehaviour
{
    public ScrollRect scrollRect;      
    public RectTransform content;      
    public int totalImages = 5;        
    public float scrollSpeed = 5f;    

    private float targetPosition = 0f;
    private int currentIndex = 0;
    private bool isScrolling = false;

    private void Start()
    {        
        currentIndex = 0;
        targetPosition = 0f;
    }

    private void Update()
    {
        if (isScrolling)
        {            
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition,
                targetPosition,
                Time.deltaTime * scrollSpeed
            );
            
            if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPosition) < 0.001f)
            {
                isScrolling = false;
            }
        }
    }

    public void ScrollNext()
    {
        if (isScrolling) return; 

        currentIndex++;

        if (currentIndex >= totalImages)
        {            
            currentIndex = 0;
            StartCoroutine(LoopScroll(1f, 0f));
            return;
        }

        targetPosition = (float)currentIndex / (totalImages - 1);
        isScrolling = true;
    }

    public void ScrollPrevious()
    {
        if (isScrolling) return;

        currentIndex--;

        if (currentIndex < 0)
        {            
            currentIndex = totalImages - 1;
            StartCoroutine(LoopScroll(0f, 1f));
            return;
        }

        targetPosition = (float)currentIndex / (totalImages - 1);
        isScrolling = true;
    }

    private IEnumerator LoopScroll(float start, float end)
    {
        isScrolling = true;
        yield return new WaitForSeconds(0.2f);
        scrollRect.horizontalNormalizedPosition = start;
        yield return new WaitForSeconds(0.05f);
        targetPosition = end;
        isScrolling = true;
    }

    public void LoadLevel()
    {        
        string[] sceneNames = { "City01", "Dungeon", "Graveyard", "Castle" }; 

        if (currentIndex >= 0 && currentIndex < sceneNames.Length)
        {
            SceneManager.LoadScene(sceneNames[currentIndex]);
        }
        else
        {
            Debug.LogError("Invalid scene index!");
        }
    }
}
