using GameFramework.WeaponSystem;
using UnityEngine;

public class FireWeapon : MonoBehaviour
{
    public GunWeapon Weapon;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            Weapon?.Attack();
        else
            Weapon?.StopAttack();

        if (Input.GetKeyDown(KeyCode.R))
            Weapon?.ReloadWeapon();
    }
}
