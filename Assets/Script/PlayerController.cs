
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private Player player;

   [SerializeField] private float slowSpeed = 0f; 
   [SerializeField] private float stopDistance = 2f; 
   [SerializeField] private float rotationAngle = 45f; 
   [Space]
   [SerializeField] private float dashSpeed = 15f; 
   [SerializeField] private float dashCooldown = 2f; 
   [SerializeField] private float dashDuration = 0.2f; 
   [SerializeField] private float rotationSpeedMultiplier = 3f;

   [Space]
   [SerializeField] private GameObject dashParticle;
   [SerializeField] private GameObject attackParticle;

    private Vector3 movement;
   
    private Vector3 targetPosition;
    public bool isDashing = false;
    private float dashTimeLeft;
    private bool isFlipped = false;

    private bool isAttacking;
    private float attackTimer;

    private bool isWalking;
    void Start()
    {
        targetPosition = transform.position;
        anim = GetComponentInChildren<Animator>();
        player = GetComponent<Player>();

    }

    void Update()
    {
        if (BuffInventory.instance.isGamePause) return;
        
        HandleState();
        HandleInput();
        HandleCooldown();
        
        MoveCharacter();
        MoveKeyboard();
    }

    private void HandleState()
    {
        anim.SetBool("isWalking", isWalking);
    }

    private void HandleCooldown()
    {
        if (attackTimer <= 1)
            attackTimer += Time.deltaTime*(player.AttackSpeed/2);
        else
            isAttacking = false;
        
        player.healthBar.InitAtkSpd(attackTimer,1);
        
    }

    void HandleInput()
    {
        if (player.IsDead()) return;
        
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0;

        if (Input.GetButtonDown("Fire2") && !isDashing)
        {
            AudioManager.instance.PlaySFX(3);
            
            Instantiate(dashParticle, this.transform.position, Quaternion.identity);
            
            isDashing = true;
            dashTimeLeft = dashDuration;
        }
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            
            AudioManager.instance.PlaySFX(Random.Range(4,7));
            //Instantiate(attackParticle, this.transform.position, Quaternion.identity);

            TriggerAttack();
            isAttacking = true;
            attackTimer = 0;

            player.Attack();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerSpecial();
        }
    }

    void MoveKeyboard()
    {
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
        transform.position += movement * player.MovementSpeed * Time.deltaTime;
    }
    void MoveCharacter()
    {
        
        if (isDashing)
        {
            Dash();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        float currentSpeed = Mathf.Lerp(slowSpeed, player.MovementSpeed, distanceToTarget / stopDistance);

        isWalking = (currentSpeed > 0.5);
        
        if (distanceToTarget < 0.1f)
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        Vector3 direction = (targetPosition - transform.position).normalized;

        //transform.position += direction * currentSpeed * Time.deltaTime;

        Vector3 localScale = transform.localScale;
        if (Mathf.Abs(direction.x) > 0.1f) 
        {
            if (direction.x < 0 && !isFlipped)
            {
                localScale.x = Mathf.Abs(localScale.x) * -1;
                isFlipped = true;
                //transform.rotation = Quaternion.identity;
            }
            else if (direction.x > 0 && isFlipped)
            {
                localScale.x = Mathf.Abs(localScale.x);
                isFlipped = false;
                //transform.rotation = Quaternion.identity;
            }
        }
        transform.localScale = localScale;

        float targetRotationZ = (isFlipped ? -rotationAngle : rotationAngle) * direction.y; 
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, targetRotationZ), Time.deltaTime * player.MovementSpeed * rotationSpeedMultiplier);
    }

    void Dash()
    {
        
        //Vector3 dashDirection = isFlipped ? -transform.right : transform.right;
        if (dashTimeLeft > 0)
        {
            transform.position += movement * dashSpeed * Time.deltaTime;
            dashTimeLeft -= Time.deltaTime;
        }
        else
        {
            isDashing = false;
        }
    }

    public void TriggerDead() => anim.SetBool("isDead", true);
    public void TriggerHurt() => anim.SetTrigger("HurtTrigger");
    public void TriggerAttack() => anim.SetTrigger("AttackTrigger");
    public void TriggerSpecial() => anim.SetTrigger("SpecialTrigger");
}
