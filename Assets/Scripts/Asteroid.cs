using UnityEngine;

namespace SpaceDodge
{
    public class Asteroid : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private GameController controller;
        private float verticalSpeed;
        private float horizontalSpeed;
        private float minX;
        private float maxX;
        private float bottomY;

        public void Setup(GameController owner, float speed, float horizontalRange, float radius, Color tint,
            float minHorizontal, float maxHorizontal, float bottom)
        {
            controller = owner;
            verticalSpeed = speed;
            horizontalSpeed = horizontalRange;
            minX = minHorizontal;
            maxX = maxHorizontal;
            bottomY = bottom;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = tint;
                spriteRenderer.size = new Vector2(radius, radius);
            }

            transform.localScale = Vector3.one * radius;
        }

        private void Update()
        {
            if (controller == null || !controller.IsRunning)
            {
                return;
            }

            var delta = Time.deltaTime;
            var pos = transform.position;
            pos.x += horizontalSpeed * delta;
            pos.y -= verticalSpeed * delta;

            if (pos.x < minX)
            {
                pos.x = minX;
                horizontalSpeed = Mathf.Abs(horizontalSpeed);
            }
            else if (pos.x > maxX)
            {
                pos.x = maxX;
                horizontalSpeed = -Mathf.Abs(horizontalSpeed);
            }

            transform.position = pos;

            if (pos.y < bottomY)
            {
                controller.HandleAsteroidPassed(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (controller == null || !controller.IsRunning)
            {
                return;
            }

            if (other.TryGetComponent(out PlayerShip ship) && !ship.IsInvulnerable)
            {
                controller.HandlePlayerHit(this);
            }
        }
    }
}
