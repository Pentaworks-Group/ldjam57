using System;

using Assets.Scripts.Scenes.Menues;

using UnityEngine;

namespace Assets.Scripts.Scenes.Credits
{
    public class CreditsBehaviour : BaseMenuBehaviour
    {
        [SerializeField]
        private KobiBehaviour Kobi;

        [SerializeField]
        private KobiBehaviour Kobi2;

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

                if (isMovingRight)
                {
                    var kobiVector = new Vector3(1, 0, 0);
                    var kobiSpawn = new Vector3(-10, Kobi.transform.position.z, 0);

                    Kobi.Fly(kobiSpawn, kobiVector);
                }
                else
                {
                    var kobiVector = new Vector3(-1, 0, 0);
                    var kobiSpawn = new Vector3(30, Kobi2.transform.position.z, 0);

                    Kobi2.Fly(kobiSpawn, kobiVector);
                }

                isMovingRight = !isMovingRight;
            }
        }
    }
}
