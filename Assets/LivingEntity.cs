using UnityEngine;

public class LivingEntity: MonoBehaviour {

    public int startingHealth;
    protected int health;

    public bool alive = false;

    protected virtual void Start(){
        health = startingHealth;
        alive = true;
    }

    public virtual void TakeDamage(int damage){
        health -= damage;
        if(health <= 0)
            Die();
    }

    protected virtual void Die(){
        alive = false;
    }
}