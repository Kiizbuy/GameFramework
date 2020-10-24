using GameFramework.WeaponSystem;
using UnityEngine;

public class FireWeapon : MonoBehaviour
{
    public GunWeapon Weapon;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Weapon?.Attack();

        if (Input.GetKeyDown(KeyCode.R))
            Weapon?.ReloadWeapon();
    }
}
