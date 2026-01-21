using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    private PowerUp powerUp;

    private void Start()
    {
        powerUp = GetComponent<PowerUp>();
    }
    private void OnTriggerEnter2D(Collider2D hit)
    {
        Player player = hit.GetComponent<Player>();
        if (player != null)
        {
            powerUp.ApplyBuff(player);
            Destroy(this.gameObject);
        }
    }
}
