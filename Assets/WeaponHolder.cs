using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public List<Weapon> weapons;
    public int activeWeaponIndex = 0;

    void Start(){
        AddAllWeapons();
    }

    public void AddAllWeapons(){
        weapons = new List<Weapon>();
        foreach(var weapon in transform.GetComponentsInChildren<Weapon>()){
            weapons.Add(weapon);
            weapon.gameObject.SetActive(false);
        }
    }

    public void CycleNextWeapon(){
        StartCoroutine(StartCycleRotate(activeWeaponIndex, (activeWeaponIndex + 1) % weapons.Count));
    }

    IEnumerator StartCycleRotate(int indexOld, int indexNew){
        weapons[indexOld].switching = true;
        weapons[indexNew].switching = true;
        //float transX = 0;
        float duration = 0.25f;
        float speed = 1 / duration * Time.deltaTime;

        float t = 0;
        var refPose = weapons[indexOld].transform.localPosition;
        float vy = weapons[indexOld].transform.localPosition.y;
        float my = weapons[indexOld].transform.localPosition.y + 0.7f;
        while(t < duration){
            t += Time.deltaTime;
            weapons[indexOld].transform.localPosition -= new Vector3(0, speed, 0);
            weapons[indexOld].transform.localPosition = new Vector3(refPose.x, Mathf.Clamp(weapons[indexOld].transform.localPosition.y, vy, my), refPose.z);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        weapons[indexOld].Unequip();
        weapons[indexOld].transform.localPosition = refPose;
        activeWeaponIndex = indexNew;

        t = 0;
        weapons[indexNew].transform.localPosition = weapons[indexOld].transform.localPosition;
        weapons[indexNew].Equip();
        while(t < duration){
            t += Time.deltaTime;
            weapons[indexNew].transform.localPosition += new Vector3(0, speed, 0);
            weapons[indexNew].transform.localPosition = new Vector3(refPose.x, Mathf.Clamp(weapons[indexNew].transform.localPosition.y, vy, my), refPose.z);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        weapons[indexNew].transform.localPosition = refPose;

        weapons[indexOld].switching = false;
        weapons[indexNew].switching = false;
    }

    public void CyclePreviousWeapon(){
        StartCoroutine(StartCycleRotate(activeWeaponIndex, activeWeaponIndex - 1 < 0 ? weapons.Count - 1 : activeWeaponIndex - 1));
    }

}
