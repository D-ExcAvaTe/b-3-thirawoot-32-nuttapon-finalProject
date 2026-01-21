using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    [SerializeField] private bool destroyOnStart = true;
    [SerializeField] private float destroyDelay = 3.0f;
    void Start()
    {
        if (destroyOnStart) StartCoroutine(DestroyDelay());
    }

    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroyed();
    }

    public void Destroyed()
    {
        Destroy(this.gameObject);
    }
}
