using UnityEngine;

public class Weapon : MonoBehaviour{
    public string name;

    public int magazineAmmoCap;
    public int currentAmmo;

    public enum FireMode { Single, Triple, Auto }
    public FireMode fireMode;

    public bool canFire = false;
    public bool switching = false;

    [Range(0.5f, 10f)]
    public float fireRate;

    protected float waitDurationPerShot = 0;    
    protected float shotTimer = 0;

    public void Init(){
        transform.localPosition = new Vector3(0.48f, -0.41f, 0.3990f);
        transform.localEulerAngles = new Vector3(-9.49f, -0.295f, 4.169f);
        transform.localScale = new Vector3(0.44029f, 0.44029f, 0.880752f);
    }

    public virtual void Start(){
        Init();
        canFire = true;
        currentAmmo = magazineAmmoCap;
        waitDurationPerShot = 1f / fireRate;
    }

    public virtual void Fire(){}

    public virtual void Unequip(){
        gameObject.SetActive(false);
    }

    public virtual void Equip(){
        gameObject.SetActive(true);
    }
}