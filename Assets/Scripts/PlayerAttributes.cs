using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAttributes : MonoBehaviour
{
    private bool isShown = false;
    private void Update()
    {
       
    }

    public void Toggle()
    {
        gameObject.SetActive(!isShown);

        isShown = !isShown;

        PlayerManager.Instance.UpdateAttributes();
    }

    public void Close()
    {
        gameObject.SetActive(false);

        isShown = false;
    }
}