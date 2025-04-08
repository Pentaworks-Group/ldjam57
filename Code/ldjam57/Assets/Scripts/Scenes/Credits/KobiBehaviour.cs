using UnityEngine;

namespace Assets.Scripts.Scenes.Credits
{
    public class KobiBehaviour : MonoBehaviour
    {
        private const float Speed = 5f;
        private Vector3 direction;

        public void Fly(Vector3 startPosition, Vector3 direction)
        {
            this.transform.position = startPosition;
            this.direction = direction;

            this.gameObject.SetActive(true);
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + direction, Speed * Time.deltaTime);
        }
    }
}
