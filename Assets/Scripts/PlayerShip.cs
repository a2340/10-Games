using UnityEngine;

namespace SpaceDodge
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerShip : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float acceleration = 18f;
        [SerializeField] private float maxSpeed = 8f;
        [SerializeField] private float damping = 12f;

        [Header("Effects")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float invulnerabilitySeconds = 1.5f;

        private float currentSpeed;
        private float minX;
        private float maxX;
        private float invulnerabilityTimer;

        public bool IsInvulnerable => invulnerabilityTimer > 0f;

        public void SetBounds(float left, float right)
        {
            minX = left;
            maxX = right;
        }

        public void ResetShip(Vector2 position)
        {
            transform.position = position;
            currentSpeed = 0f;
            invulnerabilityTimer = 0f;
            UpdateVisuals();
        }

        public void TriggerHitFeedback()
        {
            invulnerabilityTimer = invulnerabilitySeconds;
        }

        private void Update()
        {
            var delta = Time.deltaTime;
            var horizontal = Input.GetAxisRaw("Horizontal");

            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                var world = Camera.main.ScreenToWorldPoint(touch.position);
                horizontal = world.x < transform.position.x ? -1f : 1f;
            }

            currentSpeed += horizontal * acceleration * delta;
            currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

            if (Mathf.Approximately(horizontal, 0f))
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, damping * delta);
            }

            var pos = transform.position;
            pos.x += currentSpeed * delta;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;

            if (invulnerabilityTimer > 0f)
            {
                invulnerabilityTimer -= delta;
            }

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            var flicker = invulnerabilityTimer > 0f && Mathf.FloorToInt(invulnerabilityTimer * 12f) % 2 == 0;
            spriteRenderer.enabled = !flicker;
            spriteRenderer.color = IsInvulnerable ? new Color(0.6f, 0.9f, 1f, spriteRenderer.color.a) : Color.white;
        }
    }
}
