using UnityEngine;

public class Weapon : MonoBehaviour{
    public string name;

    public int magazineAmmoCap;
    public int currentAmmo;
    public int totalWeaponAmmoCapacity;

    public enum FireMode { Single, Triple, Auto }
    public FireMode fireMode;

    public bool canFire = false;
    public bool switching = false;

    [Range(0.5f, 10f)]
    public float fireRate;

    protected float waitDurationPerShot = 0;    
    protected float shotTimer = 0;

    public Vector3 equippedPosition;
    public Vector3 equippedRotation;

    public void Init(){
        transform.localPosition = equippedPosition;
        transform.localEulerAngles = equippedRotation;
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
        Init();
    }
}