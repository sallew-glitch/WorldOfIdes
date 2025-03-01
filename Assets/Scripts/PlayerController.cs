using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    
    private CharacterController _characterController;
    public Animator _animator;

    public GameObject _focusPoint;
    public GameObject _rightHandPos;
    public GameObject _leftHandPos;
    public GameObject _rightScabbardPos;
    public GameObject _leftScabbardPos;
        
    private GameObject RightMelee;
    private GameObject LeftMelee;

    [Range(1, 4)] public float MOVEMENTSPEED = 30;
    [Range(0, 0.5f)] public float GROUNDCLEARANCE;
    [Range(0, 1f)] public float GROUNDDISTANCE;
    
    private float SHIFTSPEED = 2f;
    public float GRAVITYPOWER = -9.8f;
    public float JUMPVALUE = 4f;
    public float MOUSESPEED = 10f;
    public float FALLMULTIPLIER = 2.5f; 
    public float LOWJUMPMULTIPLIER = 2f;

    private Vector3 motionVector, gravityVector;
    private Vector3 relativeVector;

    private float shiftVal = 1f;
    private float targetShiftVal = 1f;    
    private float turnDirection;
    private float jumpTimer;
    private float attackCooldown;
    private float verticalVelocity;

    public int characterState = 0;

    [HideInInspector] public bool hasHit;
    [HideInInspector] public bool isAttacking = false;

    public ABILITYSTATE physicalSkillState = ABILITYSTATE.READY;
    public ABILITYSTATE magicSkillState = ABILITYSTATE.READY;
    public ABILITYSTATE abilitySkillState = ABILITYSTATE.READY;

    private float activeTimePhysical;
    private float activeTimeMagic;
    private float activeTimeAbility;
    private float cooldownTimePhysical;
    private float cooldownTimeMagic;
    private float cooldownTimeAbility;

    private void Awake()
    {
       _characterController = GetComponent<CharacterController>();   
        _animator = GetComponent<Animator>();

        SpawnWeapon("Left");
        SpawnWeapon("Right");
    }

    public void ActivateSkill(int index)
    {
        if (index == 0)
        {
            if (physicalSkillState == ABILITYSTATE.READY)
            {
                PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex].Activate(transform);
                physicalSkillState = ABILITYSTATE.ACTIVE;
                activeTimePhysical = PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex].duration;
            }
        } else if (index == 1)
        {
            if (magicSkillState == ABILITYSTATE.READY)
            {
                PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex].Activate(transform);
                magicSkillState = ABILITYSTATE.ACTIVE;  
                activeTimeMagic = PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex].duration;
            }
        }
        else
        {
            if (abilitySkillState == ABILITYSTATE.READY)
            {
                PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex].Activate(transform);
                abilitySkillState = ABILITYSTATE.ACTIVE;
                activeTimeAbility = PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex].duration;

            }
        }
        characterState = 0;
    }

    public void MonitorSkillState()
    {
        switch (physicalSkillState)
        {
            case ABILITYSTATE.READY:
                break;
            case ABILITYSTATE.ACTIVE:
                if (activeTimePhysical > 0)                
                    activeTimePhysical -= Time.deltaTime;
                else
                {
                    //PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex].Cooldown(transform);
                    physicalSkillState = ABILITYSTATE.COOLDOWN;
                    cooldownTimePhysical = PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex].cooldown;
                    HUDManager.Instance.SkillHUDCooldown(SKILLTYPE.PHYSICAL, PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex], cooldownTimePhysical);
                }
                break;
            case ABILITYSTATE.COOLDOWN:
                if (cooldownTimePhysical > 0)             
                    cooldownTimePhysical -= Time.deltaTime;
                 else
                {
                    HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.PHYSICAL, PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex]);
                    physicalSkillState = ABILITYSTATE.READY;                
                }
                break;
        }

        switch (magicSkillState)
        {
            case ABILITYSTATE.READY:
                break;
            case ABILITYSTATE.ACTIVE:
                if (activeTimeMagic > 0)
                    activeTimeMagic -= Time.deltaTime;
                else
                {
                    //PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex].Cooldown(transform);
                    magicSkillState = ABILITYSTATE.COOLDOWN;
                    cooldownTimeMagic = PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex].cooldown;
                    HUDManager.Instance.SkillHUDCooldown(SKILLTYPE.MAGIC, PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex], cooldownTimeMagic);
                }
                break;
            case ABILITYSTATE.COOLDOWN:
                if (cooldownTimeMagic > 0)
                    cooldownTimeMagic -= Time.deltaTime;
                else
                {
                    HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.MAGIC, PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex]);
                    magicSkillState = ABILITYSTATE.READY;
                }
                break;
        }

        switch (abilitySkillState)
        {
            case ABILITYSTATE.READY:
                break;
            case ABILITYSTATE.ACTIVE:
                if (abilitySkillState > 0)
                    activeTimeAbility -= Time.deltaTime;
                else
                {
                    //PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex].Cooldown(transform);
                    abilitySkillState = ABILITYSTATE.COOLDOWN;
                    cooldownTimeAbility = PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex].cooldown;
                    HUDManager.Instance.SkillHUDCooldown(SKILLTYPE.ABILITY, PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex], cooldownTimeAbility);
                }
                break;
            case ABILITYSTATE.COOLDOWN:
                if (cooldownTimeAbility > 0)
                    cooldownTimeAbility -= Time.deltaTime;
                else
                {
                    HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.ABILITY, PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex]);
                    abilitySkillState = ABILITYSTATE.READY;
                }
                break;
        }
    }

    public void PlayerControl()
    {
        if (InputManager.Instance._inputController.LeftShiftInput())
            targetShiftVal = 2;
        else
            targetShiftVal = 1;

        shiftVal = Mathf.Lerp(shiftVal, targetShiftVal, Time.deltaTime * SHIFTSPEED);
        shiftVal = Mathf.Clamp(shiftVal, 1f, 2f);        

        if (PlayerManager.Instance.PlayerStatus == PLAYERSTATUS.ALIVE)
        {
            PlayerMovement();
            Animations();
            MonitorSkillState();

            if (InputManager.Instance._inputController.HKeyInput())
            {
                ChangeCharState();
            }
        }

        MouseMovement();        
    }

    private void SpawnWeapon(string choice)
    {
        if (choice == "Right")
        {            
            RightMelee = PlayerManager.Instance.PlayerWeaponsRight[PlayerManager.Instance.currentWeaponRight].Instantiate();
            RightMelee.transform.parent = _rightScabbardPos.transform;
            RightMelee.transform.localPosition = Vector3.zero;
            RightMelee.transform.localEulerAngles = Vector3.zero;
            RightMelee.transform.localScale = new Vector3(1, 1, 1);
        }
       else if (choice == "Left")
        {
            LeftMelee = PlayerManager.Instance.PlayerWeaponsLeft[PlayerManager.Instance.currentWeaponLeft].Instantiate();
            LeftMelee.transform.parent = _leftScabbardPos.transform;
            LeftMelee.transform.localPosition = Vector3.zero;
            LeftMelee.transform.localEulerAngles = Vector3.zero;
            LeftMelee.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void EquipMelee()
    {
        RightMelee.transform.parent = _rightHandPos.transform;
        RightMelee.transform.localPosition = Vector3.zero;
        RightMelee.transform.localEulerAngles = Vector3.zero;
        RightMelee.transform.localScale = new Vector3(1, 1, 1);

        LeftMelee.transform.parent = _leftHandPos.transform;
        LeftMelee.transform.localPosition = Vector3.zero;
        LeftMelee.transform.localEulerAngles = Vector3.zero;
        LeftMelee.transform.localScale = new Vector3(1, 1, 1);
    }

    void UnEquipMelee()
    {        
        RightMelee.transform.parent = _rightScabbardPos.transform;
        RightMelee.transform.localPosition = Vector3.zero;
        RightMelee.transform.localEulerAngles = Vector3.zero;
        RightMelee.transform.localScale = new Vector3(1, 1, 1);

        LeftMelee.transform.parent = _leftScabbardPos.transform;
        LeftMelee.transform.localPosition = Vector3.zero;
        LeftMelee.transform.localEulerAngles = Vector3.zero;
        LeftMelee.transform.localScale = new Vector3(1, 1, 1);       
    }

    private void PlayerMovement()
    {
        if (IsGrounded() && gravityVector.y < 0)
            gravityVector.y = -2;

        gravityVector.y += GRAVITYPOWER * Time.deltaTime;
        
        _characterController.Move(gravityVector * Time.deltaTime);  

  
        motionVector = transform.right * (InputManager.Instance.Horizontal) + transform.forward * InputManager.Instance.Vertical;

        if (InputManager.Instance.MOVEMENT)
        {
            _characterController.Move(motionVector * MOVEMENTSPEED * shiftVal * Time.deltaTime);
            transform.Rotate(transform.up * turnDirection * 400 * Time.deltaTime);
            _focusPoint.transform.parent.Rotate(transform.up * -turnDirection * 400 * Time.deltaTime);
        }
        
        if (InputManager.Instance.Jump != 0)
        {
            Jump();
        }        
    }

    public void TakeDamage(float damage)
    {        

        if (!_animator.GetBool("shield"))
        {
            if (PlayerManager.Instance.playerHP > damage )
            {
                _animator.SetTrigger("takeDamage");
                PlayerManager.Instance.UpdateHealth(PlayerManager.Instance.playerHP - damage);
            }
            else
            {
                PlayerManager.Instance.PlayerStatus = PLAYERSTATUS.DEAD;
                PlayerManager.Instance.UpdateHealth(0);
                Die();
            }
        }
        else
        {
            _animator.SetTrigger("blockHit");
        }
    }

    public void Die()
    {        
        StartCoroutine(RespawnPlayer(5f));        
    }

    private IEnumerator RespawnPlayer(float time)
    {
        yield return new WaitForSeconds(time);

        PlayerManager.Instance.transform.position = PlayerManager.Instance._spawnPoint.transform.position;
        PlayerManager.Instance.PlayerStatus = PLAYERSTATUS.ALIVE;
        PlayerManager.Instance.UpdateHealth(PlayerManager.Instance.maxHP);    
    }

    private void MouseMovement()
    {
        if (InputManager.Instance.CURSORLOCKED)
        {
            relativeVector = transform.InverseTransformPoint(_focusPoint.transform.position);
            relativeVector /= relativeVector.magnitude;

            turnDirection = (relativeVector.x / relativeVector.magnitude);
            
            _focusPoint.transform.eulerAngles = new Vector3(_focusPoint.transform.eulerAngles.x + -Input.GetAxis("Mouse Y"), _focusPoint.transform.eulerAngles.y, 0);
            _focusPoint.transform.parent.Rotate(transform.up * Input.GetAxis("Mouse X") * MOUSESPEED * Time.deltaTime);
        }
    }

    private void Animations()
    {
        _animator.SetBool("grounded", IsGrounded());
        _animator.SetFloat("health", PlayerManager.Instance.playerHP);
        _animator.SetFloat("vertical", InputManager.Instance.Vertical * shiftVal);
        _animator.SetFloat("horizontal", (InputManager.Instance.Vertical != 0 ? InputManager.Instance.Horizontal / 2 : InputManager.Instance.Horizontal) * shiftVal);

        Debug.DrawRay(RightMelee.transform.position, RightMelee.transform.up, Color.red);

        if (IsGrounded() && Time.time > jumpTimer)
            _animator.SetFloat("jump", InputManager.Instance.Jump);

        _animator.SetInteger("state", characterState);

        //Restrict movement until attack animation is finished
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("meleeAttack"))
        {
            isAttacking = true;
        } 
        else
        {
            isAttacking = false;
        }

        if (InputManager.Instance.Fire1 != 0 && InputManager.Instance.Fire2 == 0 && Time.time > attackCooldown && characterState == 1)
        {
            _animator.SetTrigger("attack");
            attackCooldown = Time.time + 1.4f;
            hasHit = false;
            PlayerManager.Instance.PlayerWeaponsRight[PlayerManager.Instance.currentWeaponRight].Attack(RightMelee);
        }

        if (InputManager.Instance.Fire2 != 0 && InputManager.Instance.Fire1 == 0)
        {
            _animator.SetBool("shield", true);
        }
        else
        {
            _animator.SetBool("shield", false);
        }
    }

    private async void Jump()
    {
        if (IsGrounded() && Time.time > jumpTimer)
        {
            await Task.Delay(500);

            _characterController.Move(transform.up * (JUMPVALUE * -1 * GRAVITYPOWER) * Time.deltaTime);
            jumpTimer = Time.time + 4;
        }

    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - GROUNDDISTANCE, transform.position.z), GROUNDCLEARANCE);
    }

    string GetCharState()
    {
        switch (characterState)
        {
            case 0:
                return "peaceful";

            case 1:
                return "combat";

        }

        return "out of range";
    }

    void ChangeCharState()
    {
        if (characterState == 1)
        {
            characterState = 0;
            UnEquipMelee();
        }
        else
        {
            characterState = 1;
            EquipMelee();
        }
    }

    public void recieveDamage(float damage)
    {
        if (!_animator.GetBool("shield"))
        {
            if (PlayerManager.Instance.playerHP > damage)
                _animator.SetTrigger("takeDamage");
            
            PlayerManager.Instance.UpdateHealth(PlayerManager.Instance.playerHP - damage);            
        }
        else
        {
           _animator.SetTrigger("blockHit");
        }
    }

    public void CycleSkill(SKILLTYPE skillType)
    {
        switch (skillType)
        {
            case SKILLTYPE.PHYSICAL:
                PlayerManager.Instance.currentPhysicalIndex = (PlayerManager.Instance.currentPhysicalIndex + 1) % PlayerManager.Instance.PlayerPhysicalSkills.Count;
                HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.PHYSICAL, PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex]);
                break;
            case SKILLTYPE.MAGIC:
                PlayerManager.Instance.currentMagicIndex = (PlayerManager.Instance.currentMagicIndex + 1) % PlayerManager.Instance.PlayerMagicalSkills.Count;
                HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.MAGIC, PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex]);
                break;
            case SKILLTYPE.ABILITY:
                PlayerManager.Instance.currentAbilityIndex = (PlayerManager.Instance.currentAbilityIndex + 1) % PlayerManager.Instance.PlayerAbilitySkills.Count;
                HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.ABILITY, PlayerManager.Instance.PlayerAbilitySkills[PlayerManager.Instance.currentAbilityIndex]);
                break;
        }
    }

    public void CycleWeapon(WEAPONTYPE weaponType)
    {
        switch (weaponType)
        {
            case WEAPONTYPE.LEFT:
                if ((PlayerManager.Instance.currentWeaponLeft + 1) % PlayerManager.Instance.PlayerWeaponsLeft.Count == PlayerManager.Instance.currentWeaponLeft)
                    break;
                PlayerManager.Instance.currentWeaponLeft = (PlayerManager.Instance.currentWeaponLeft + 1) % PlayerManager.Instance.PlayerWeaponsLeft.Count;
                Destroy(LeftMelee);
                characterState = 0;
                UnEquipMelee();
                SpawnWeapon("Left");
                //HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.PHYSICAL, PlayerManager.Instance.PlayerPhysicalSkills[PlayerManager.Instance.currentPhysicalIndex]);
                break;
            case WEAPONTYPE.RIGHT:
                if ((PlayerManager.Instance.currentWeaponRight + 1) % PlayerManager.Instance.PlayerWeaponsRight.Count == PlayerManager.Instance.currentWeaponRight)
                    break;
                PlayerManager.Instance.currentWeaponRight = (PlayerManager.Instance.currentWeaponRight + 1) % PlayerManager.Instance.PlayerWeaponsRight.Count;
                Destroy(RightMelee);
                characterState = 0;
                UnEquipMelee();
                SpawnWeapon("Right");
                //HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.MAGIC, PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex]);
                break;
        }
    }

    public void OnDrawGizmosSelected()
    {
        Vector3 center = GameObject.FindGameObjectWithTag("attackPos").transform.position;
        Gizmos.DrawSphere(center, 1f);
    }
}
