using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance { get { return _instance; } }

    public InputController _inputController;

    [HideInInspector] public float Vertical, Horizontal, Jump, Fire1, Fire2 ;
    [HideInInspector] public bool INPUTRECEIVED;
    [HideInInspector] public bool MOVEMENT;
    [HideInInspector] public bool JUMP;
    [HideInInspector] public bool CURSORLOCKED;

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
        DontDestroyOnLoad(gameObject);

        _inputController = GetComponent<InputController>();
    }
    
    public void UpdateMovement(float Vertical, float Horizontal, float Jump, float Fire1, float Fire2)
    {
        
    }    
}
