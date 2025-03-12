using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScrollViewController : MonoBehaviour
{
    public ScrollRect scrollRect;      // Assign the ScrollRect
    public RectTransform content;      // Assign the content (where images are inside)
    public int totalImages = 5;        // Number of images in the scroll view
    public float scrollSpeed = 5f;     // Speed of smooth scrolling

    private float targetPosition = 0f;
    private int currentIndex = 0;
    private bool isScrolling = false;

    private void Start()
    {
        // Ensure scrolling starts at the first image
        currentIndex = 0;
        targetPosition = 0f;
    }

    private void Update()
    {
        if (isScrolling)
        {
            // Smoothly move towards the target position
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition,
                targetPosition,
                Time.deltaTime * scrollSpeed
            );

            // Stop scrolling when close to the target
            if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPosition) < 0.001f)
            {
                isScrolling = false;
            }
        }
    }

    public void ScrollNext()
    {
        if (isScrolling) return; // Prevent spamming

        currentIndex++;

        if (currentIndex >= totalImages)
        {
            // Loop back to first image
            currentIndex = 0;
            StartCoroutine(LoopScroll(1f, 0f));
            return;
        }

        targetPosition = (float)currentIndex / (totalImages - 1);
        isScrolling = true;
    }

    public void ScrollPrevious()
    {
        if (isScrolling) return; // Prevent spamming

        currentIndex--;

        if (currentIndex < 0)
        {
            // Loop to last image
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
        yield return new WaitForSeconds(0.2f); // Short delay before snapping
        scrollRect.horizontalNormalizedPosition = start; // Instantly snap
        yield return new WaitForSeconds(0.05f); // Small delay
        targetPosition = end;
        isScrolling = true;
    }

    public void LoadLevel()
    {
        // Assign scene names based on the current index
        string[] sceneNames = { "City01", "Dungeon", "Graveyard", "MainMenu" }; // Change based on your actual scene names

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
