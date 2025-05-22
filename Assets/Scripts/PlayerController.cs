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
    public SkillExecutioner _skillExecutioner;

    public GameObject _focusPoint;
    public GameObject _rightHandPos;
    public GameObject _leftHandPos;
    public GameObject _rightScabbardPos;
    public GameObject _leftScabbardPos;
        
    private GameObject RightMelee;
    private GameObject LeftMelee;

    [Range(1, 4)] public float MOVEMENTSPEED = 30;
    [Range(0, 0.5f)] public float GROUNDCLEARANCE;
    [Range(0, 2f)] public float GROUNDDISTANCE;
    
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
    private float parryCooldown;
    private float verticalVelocity;

    private float dodgeCooldown;

    public int characterState = 0;

    private bool oneHanded = false;

    [HideInInspector] public bool hasHit;
    [HideInInspector] public bool isAttacking = false;

    private void Awake()
    {
        _skillExecutioner = GetComponent<SkillExecutioner>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {     
        SpawnWeapon("Left");
        SpawnWeapon("Right");
        oneHanded = _animator.GetInteger("weaponId") < 4 ? true : false;
    }

    public void ActivateSkill(int index)
    {
        if (index == 0)
        {
            _skillExecutioner.CastPhysicalSkill();
        } else if (index == 1)
        {
            _skillExecutioner.CastMagicalSkill();
        }
        else
        {
            _skillExecutioner.CastAbilitySkill();
        }
        //characterState = 0;
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
            //SkillManager.Instance.MonitorSkillState();
            PlayerManager.Instance.ManaRegen(10);
            PlayerManager.Instance.StaminaRegen(10);

            if (InputManager.Instance._inputController.HKeyInput())
            {
                ChangeCharState();
            }

            if (InputManager.Instance._inputController.ActivateKeyInput())
            {
                ActivateStuff();
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
        // Use our improved IsGrounded check
        if (IsGrounded() && gravityVector.y < 0)
        {
            // When grounded, we set a small downward force to keep contact.
            gravityVector.y = -2f;
        }
        else
        {
            // Otherwise, apply gravity.
            gravityVector.y += GRAVITYPOWER * Time.deltaTime;
        }

        // Apply vertical movement (gravity)
        _characterController.Move(gravityVector * Time.deltaTime);

        // Calculate horizontal motion
        motionVector = transform.right * InputManager.Instance.Horizontal + transform.forward * InputManager.Instance.Vertical;

        if (InputManager.Instance.MOVEMENT)
        {
            _characterController.Move(motionVector * MOVEMENTSPEED * shiftVal * Time.deltaTime);
            transform.Rotate(transform.up * turnDirection * 400f * Time.deltaTime);
            _focusPoint.transform.parent.Rotate(transform.up * -turnDirection * 400f * Time.deltaTime);
        }

        if (InputManager.Instance.Jump != 0)
        {
            Jump();
        }
    }


    public void TakeDamage(float damage)
    {        

        if (!_animator.GetBool("shield")) //not blocked
        {
            if (PlayerManager.Instance.playerHP > damage )
            {
                _animator.SetTrigger("takeDamage" + Random.Range(1,4));
                PlayerManager.Instance.UpdateHealth(PlayerManager.Instance.playerHP - damage);
            }
            else
            {
                PlayerManager.Instance.PlayerStatus = PLAYERSTATUS.DEAD;
                PlayerManager.Instance.UpdateHealth(0);
                _animator.SetFloat("health", 0);
                Die();
            }

            Debug.Log("Health : " + PlayerManager.Instance.playerHP);
        }
        else //blocked
        {
            if (PlayerManager.Instance.playerStamina > 0)
            {
                _animator.SetTrigger("blockHit" + Random.Range(1,3));

                //block audio
                int index = Random.Range(1, 6);
                AudioManager.instance.Play("Block" + index);

                PlayerManager.Instance.UpdateStamina(PlayerManager.Instance.playerStamina - damage);
            }
        }
    }

    public void Die()
    {
        if (oneHanded)
            _animator.SetTrigger("death" + Random.Range(1, 3));
        else
            _animator.SetTrigger("death1");
        StartCoroutine(RespawnPlayer(5f));        
    }

    private IEnumerator RespawnPlayer(float time)
    {
        yield return new WaitForSeconds(time);

        PlayerManager.Instance.transform.position = PlayerManager.Instance._spawnPoint.transform.position;
        PlayerManager.Instance.PlayerStatus = PLAYERSTATUS.ALIVE;
        PlayerManager.Instance.UpdateHealth(PlayerManager.Instance.maxHP);
        _animator.SetFloat("health", PlayerManager.Instance.maxHP);
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

        //Debug.DrawRay(RightMelee.transform.position, RightMelee.transform.up, Color.red);

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

        if (Time.time > dodgeCooldown) // dodges
        {
            if (InputManager.Instance.DodgeF && !InputManager.Instance.Parry && !InputManager.Instance.LightAttack && !InputManager.Instance.HeavyAttack)
            {
                _animator.SetTrigger("dodgeF");
                dodgeCooldown = Time.time + 0.5f;
            }

            if (InputManager.Instance.DodgeB && !InputManager.Instance.Parry && !InputManager.Instance.LightAttack && !InputManager.Instance.HeavyAttack)
            {
                _animator.SetTrigger("dodgeB");
                dodgeCooldown = Time.time + 0.5f;
            }

            if (InputManager.Instance.DodgeL && !InputManager.Instance.Parry && !InputManager.Instance.LightAttack && !InputManager.Instance.HeavyAttack)
            {
                _animator.SetTrigger("dodgeL");
                dodgeCooldown = Time.time + 0.5f;
            }

            if (InputManager.Instance.DodgeR && !InputManager.Instance.Parry && !InputManager.Instance.LightAttack && !InputManager.Instance.HeavyAttack)
            {
                _animator.SetTrigger("dodgeR");
                dodgeCooldown = Time.time + 0.5f;
            }
        }

        if (!InputManager.Instance.Block && Time.time > attackCooldown)
        {
            if (characterState == 1) //equipped attack
            {
                if (InputManager.Instance.LightAttack && !InputManager.Instance.HeavyAttack) //light attack
                {
                    _animator.SetTrigger("lightAttack");
                    attackCooldown = Time.time + 1.4f;
                    hasHit = false;
                    PlayerManager.Instance.PlayerWeaponsRight[PlayerManager.Instance.currentWeaponRight].LightAttack(RightMelee);
                }
                else if (!InputManager.Instance.LightAttack && InputManager.Instance.HeavyAttack) //heavy attack
                {
                    _animator.SetTrigger("heavyAttack");

                    if (_animator.GetInteger("weaponId") == 7) // staff spells
                    {
                        StartCoroutine(ExecuteHeavySpell());
                    }
                    else // all other heavy attacks
                    {
                        attackCooldown = Time.time + 1.7f;
                        hasHit = false;
                        PlayerManager.Instance.PlayerWeaponsRight[PlayerManager.Instance.currentWeaponRight].HeavyAttack(RightMelee);
                    }
                    
                }
            }
            else //equip weapon first
            {
                if (InputManager.Instance.LightAttack || InputManager.Instance.HeavyAttack) //light attack
                {
                    ChangeCharState();
                }
            }
        }

        if (InputManager.Instance.Block && !InputManager.Instance.LightAttack && !InputManager.Instance.HeavyAttack && !InputManager.Instance.Parry) //block
        {
            _animator.SetBool("shield", true);
            MOVEMENTSPEED = 1;
        }
        else
        {
            _animator.SetBool("shield", false);
            MOVEMENTSPEED = 2;
        }

        if (InputManager.Instance.Parry && !InputManager.Instance.LightAttack && !InputManager.Instance.HeavyAttack && !InputManager.Instance.Block && Time.time > parryCooldown)
        {
            _animator.SetTrigger("spAttack2");

            if (_animator.GetInteger("weaponId") == 7) // staff spells
            {
                StartCoroutine(ExecuteSpecialSpell2());
            }
            else // all other parries or special attacks
            {
                parryCooldown = Time.time + 2f;
                //PlayerManager.Instance.PlayerWeaponsLeft[PlayerManager.Instance.currentWeaponLeft].HeavyAttack(LeftMelee);
            }
        }
    }

    private IEnumerator ExecuteHeavySpell()
    {
        // spell logic
        yield return new WaitForSeconds(3f);
        _animator.SetTrigger("heavySpellEnd");
    }

    private IEnumerator ExecuteSpecialSpell1()
    {
        // spell logic
        yield return new WaitForSeconds(4f);
        _animator.SetTrigger("spSpell1End");
    }

    private IEnumerator ExecuteSpecialSpell2()
    {
        // spell logic
        yield return new WaitForSeconds(4f);
        _animator.SetTrigger("spSpell2End");
    }

    private async void Jump()
    {
        if (IsGrounded() && Time.time > jumpTimer)
        {

            await System.Threading.Tasks.Task.Delay(100);

            _characterController.Move(transform.up * (JUMPVALUE * -1 * GRAVITYPOWER) * Time.deltaTime);
            jumpTimer = Time.time + 4;
        }
    }

    private bool IsGrounded()
    {
        Vector3[] offsets = { Vector3.zero,
                          transform.right * _characterController.radius * 0.9f,
                         -transform.right * _characterController.radius * 0.9f };

        foreach (var ofs in offsets)
        {
            Vector3 origin = transform.position + Vector3.up * 0.1f + ofs;
            if (Physics.Raycast(origin, Vector3.down,
                                out RaycastHit hit,
                                GROUNDDISTANCE + 0.1f))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) <= 60)
                    return true;
            }
        }
        return false;
    }


/*    private bool IsGrounded()
    {
        RaycastHit hit;       
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        
        float rayDistance = GROUNDDISTANCE + 0.1f;
        
        if (Physics.Raycast(origin, Vector3.down, out hit, rayDistance))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle < 60f;
        }
        return false;
    }
*/
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

    void ActivateStuff()
    {
        if (oneHanded)
            _animator.SetTrigger("activate" + Random.Range(1, 3));
        else
            _animator.SetTrigger("activate1");
    }

    public void ChangeWeapon(int weaponIndex,string weaponType)
    {
        Debug.Log("[Player Controller] Inside Change Weapon");

        switch (weaponType)
        {
            case "Left":

                PlayerManager.Instance.currentWeaponLeft = weaponIndex;
                Destroy(LeftMelee);
                characterState = 0;
                UnEquipMelee();
                SpawnWeapon("Left");
                break;
            case "Right":
                
                PlayerManager.Instance.currentWeaponRight = weaponIndex;
                Destroy(RightMelee);
                characterState = 0;
                UnEquipMelee();
                SpawnWeapon("Right");
                //HUDManager.Instance.UpdateSkillHolderUI(SKILLTYPE.MAGIC, PlayerManager.Instance.PlayerMagicalSkills[PlayerManager.Instance.currentMagicIndex]);
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
