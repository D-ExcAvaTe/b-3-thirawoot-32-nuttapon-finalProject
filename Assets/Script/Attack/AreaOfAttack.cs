using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaOfAttack : MonoBehaviour
{
    protected float damage;
    protected Character owner;
    [SerializeField] private bool destroyOnStart = true;
    [SerializeField] float destroyDelay = 0.5f;
    public void Init(float _damage, Character _owner) 
    {
        damage = _damage;
        owner = _owner;
    }

    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
    private void Start()
    {
        if(destroyOnStart)
            StartCoroutine(DestroyDelay());
    }
    public abstract void OnHitWithCharacter(Character target);
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Character>() != null)
        {
            OnHitWithCharacter(other.GetComponent<Character>());
        }
    }
}
