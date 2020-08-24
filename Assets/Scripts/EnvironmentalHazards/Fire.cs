using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : EnvironmentalHazard
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Fire"))
        {
            Destroy(this.gameObject);
        }
        else if (collider.gameObject.CompareTag("Enemy") || collider.gameObject.CompareTag("Player"))
        {
            foreach (StatusEffect effect in effects)
            {
                collider.gameObject.GetComponent<MovingObject>().ApplyStatus(Object.Instantiate(effect));
            }
            collider.gameObject.GetComponent<MovingObject>().TakeDamage(damage);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {

    }
}
