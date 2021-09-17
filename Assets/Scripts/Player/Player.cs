using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public string message;

    [SerializeField] private GameObject carbineObj;
    [SerializeField] private GameObject smgObj;
    [SerializeField] private GameObject weaponsObj;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
    }

    public void UpdatePosition(Vector3 _position, Quaternion _rotation)
    {
        transform.position = _position;
        transform.localRotation = _rotation;

        ServerSend.PlayerPositionToAll(id, transform.position, transform.rotation);
    }

    public void ActivateWeapon(int _weaponId)
    {
        //Deactivate other weapons
        carbineObj.SetActive(false);
        smgObj.SetActive(false);

        //Activate chosen weapon
        GameObject weapon;

        switch(_weaponId)
        {
            case 0:
                weapon = carbineObj;
                break;
            case 1:
                weapon = smgObj;
                break;
            default:
                weapon = carbineObj;
                break;
        }

        weapon.SetActive(true);
        ServerSend.PlayerWeapon(id, _weaponId);
    }

    public void WeaponsRotation(Quaternion _rotation)
    {
        weaponsObj.transform.localRotation = _rotation;

        ServerSend.PlayerWeaponRotation(id, weaponsObj.transform.localRotation);
    }
}

