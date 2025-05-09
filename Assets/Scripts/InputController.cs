using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{  
    public void InputControl()
    {
        PauseGameInput();
        if (!GameManager.Instance.PAUSED)
        {
            MovementInput();
            CursorLockInput();
            LeftWeaponChangeKeyInput();
            RightWeaponChangeKeyInput();
            PhysicalSkillKeyInput();
            MagicSkillKeyInput();
            AbilitySkillKeyInput();
            PhysicalSkillChangeKeyInput();
            MagicSkillChangeKeyInput();
            AbilitySkillChangeKeyInput();

        }
        SaveGame();
        LoadGame();
    }

    public void PauseGameInput()    
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuController menu = FindObjectOfType<MenuController>();
            if (menu != null)
            {
                if (GameManager.Instance.PAUSED)
                    menu.ResumeGame();
                else
                    menu.PauseGame();
            }
        }
    }

    public void MovementInput()
    {
        InputManager.Instance.Vertical = Input.GetAxis("Vertical");
        InputManager.Instance.Horizontal = Input.GetAxis("Horizontal");
        InputManager.Instance.Jump = Input.GetAxis("Jump");

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetAxis("Fire1") != 0)
            {
                InputManager.Instance.LightAttack = true;
                Debug.Log("InputManager LightAttack");
            }
            else
            {
                InputManager.Instance.LightAttack = false;
            }

            if (Input.GetAxis("Fire2") != 0) //Block
            {
                InputManager.Instance.Block = true;
                Debug.Log("InputManager Block");
            }
            else
            {
                InputManager.Instance.Block = false;
            }

        } 
        else
        {
            if (Input.GetAxis("Fire1") != 0) //Heavy Attack
            {
                InputManager.Instance.HeavyAttack = true;
                Debug.Log("InputManager HeavyAttack");
            }
            else
            {
                InputManager.Instance.HeavyAttack = false;
            }

            if (Input.GetAxis("Fire2") != 0) //Parry
            {
                InputManager.Instance.Parry = true;
                Debug.Log("InputManager Parry");
            } 
            else
            {
                InputManager.Instance.Parry = false;
            }
        }

        
        if (InputManager.Instance.Vertical != 0f || InputManager.Instance.Horizontal != 0f || InputManager.Instance.Jump != 0f || InputManager.Instance.LightAttack || InputManager.Instance.HeavyAttack)        
            InputManager.Instance.INPUTRECEIVED = true;        
        else        
            InputManager.Instance.INPUTRECEIVED = false;        

        if ((InputManager.Instance.Vertical != 0f || InputManager.Instance.Horizontal != 0f) && !PlayerManager.Instance._playerController.isAttacking)        
            InputManager.Instance.MOVEMENT = true;
        else
            InputManager.Instance.MOVEMENT = false;

        if (Input.GetKeyDown(KeyCode.Space))        
            InputManager.Instance.JUMP = true;
        else
            InputManager.Instance.JUMP = false;
    }

    public void CursorLockInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) InputManager.Instance.CURSORLOCKED = InputManager.Instance.CURSORLOCKED ? false : true;
        Cursor.lockState = InputManager.Instance.CURSORLOCKED ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public bool LeftShiftInput()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }

    public bool HKeyInput()
    {
        return Input.GetKeyDown(KeyCode.H);
    }

    public bool ActivateKeyInput()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    public void LeftWeaponChangeKeyInput()
    {
         if (Input.GetKeyDown(KeyCode.Alpha1))
            PlayerManager.Instance._playerController.CycleWeapon(WEAPONTYPE.LEFT);

    }
    public void RightWeaponChangeKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
            PlayerManager.Instance._playerController.CycleWeapon(WEAPONTYPE.RIGHT);
    }    
    public void PhysicalSkillKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
            HUDManager.Instance.SkillButtons[0].onClick.Invoke();
    }

    public void MagicSkillKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
            HUDManager.Instance.SkillButtons[1].onClick.Invoke();
    }

    public void AbilitySkillKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.V))
            HUDManager.Instance.SkillButtons[2].onClick.Invoke();
    }   

    public void PhysicalSkillChangeKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SkillManager.Instance.CycleSkill(SKILLTYPE.PHYSICAL);
    }

    public void MagicSkillChangeKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SkillManager.Instance.CycleSkill(SKILLTYPE.MAGIC);
    }

    public void AbilitySkillChangeKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
            SkillManager.Instance.CycleSkill(SKILLTYPE.ABILITY);
    }

    public void SaveGame()
    {
        if (Input.GetKeyDown(KeyCode.F5)) // Save
            SaveManager.Instance.SaveGame();        
    }

    public void LoadGame()
    {
        if (Input.GetKeyDown(KeyCode.F9)) // Load
            SaveManager.Instance.LoadGame();
    }

}
