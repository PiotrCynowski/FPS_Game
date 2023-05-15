using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PowerUpObj : MonoBehaviour, IAmPowerup
{
    [SerializeField] private typeOfPowerup powerupType;
    [SerializeField] private int powerDuration;

    [SerializeField] private float respawnTime;

    int IAmPowerup.powerDuration => powerDuration;

    public typeOfPowerup PowerUpType()
    {
        return powerupType;
    }
}


public enum typeOfPowerup { forWeapon, forPlayerMovement };

