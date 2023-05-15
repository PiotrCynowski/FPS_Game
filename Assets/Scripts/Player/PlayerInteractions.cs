using UnityEngine;

namespace Player
{
    public class PlayerInteractions : MonoBehaviour
    {
        public delegate void OnPowerupWeaponBoost(int _puDuration);
        public static event OnPowerupWeaponBoost onWeaponBoost;

        public delegate void OnPowerupMovementBoost(int _puDuration);
        public static event OnPowerupMovementBoost onMovementBoost;


        public void OnTriggerEnter(Collider _other)
        {

            if (_other.gameObject.GetComponent<ICanParentPlayer>() != null)
            {
                transform.parent = _other.gameObject.transform;
                return;
            }

            IAmPowerup powerup = _other.gameObject.GetComponent<IAmPowerup>();

            if (powerup != null)
            {
                UsePowerUp(powerup);
                Destroy(_other.gameObject);
            }
        }

        public void OnTriggerExit(Collider _other)
        {
            transform.parent = null;
        }

        private void UsePowerUp(IAmPowerup _powerup)
        {
            switch (_powerup.PowerUpType())
            {
                case typeOfPowerup.forWeapon:
                    onWeaponBoost?.Invoke(_powerup.powerDuration);
                    break;
                case typeOfPowerup.forPlayerMovement:
                    onMovementBoost?.Invoke(_powerup.powerDuration);
                    break;
                default:
                    break;
            }
        }
    }
}


interface ICanParentPlayer { }

interface IAmPowerup 
{
    int powerDuration { get; }
    typeOfPowerup PowerUpType();
}

