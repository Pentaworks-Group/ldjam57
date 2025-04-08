using System;

using UnityEngine;

namespace Assets.Scripts.Scenes.Credits
{
    public class CreditsBehaviour : MonoBehaviour
    {
        [SerializeField]
        private KobiBehaviour Kobi;

        private float flyoutTrigger = 10;
        private float interval = 10;
        private Boolean isMovingRight = true;

        private void Update()
        {
            if (flyoutTrigger > 0)
            {
                flyoutTrigger -= Time.deltaTime;
            }
            else
            {
                flyoutTrigger = interval;

                var kobiVector = new Vector3(1, 0, 0);

                if (isMovingRight)
                {

                    var kobiSpawn = new Vector3(-10, Kobi.transform.position.z, 0);
                }

                //Kobi.Fly(kobiSpawn, kobiVector);
            }
        }
    }
}
