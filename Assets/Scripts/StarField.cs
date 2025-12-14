using UnityEngine;

namespace SpaceDodge
{
    public class StarField : MonoBehaviour
    {
        [SerializeField] private Transform[] stars;
        [SerializeField] private Vector2 speedRange = new Vector2(0.4f, 1.6f);

        private float topY;
        private float bottomY;
        private float minX;
        private float maxX;
        private float[] speeds;

        public void Configure(Camera cam, float padding)
        {
            if (cam == null || stars == null || stars.Length == 0)
            {
                return;
            }

            var halfHeight = cam.orthographicSize;
            var halfWidth = halfHeight * cam.aspect;
            topY = halfHeight + 0.5f;
            bottomY = -halfHeight - 0.5f;
            minX = -halfWidth + padding;
            maxX = halfWidth - padding;

            speeds = new float[stars.Length];
            for (var i = 0; i < stars.Length; i++)
            {
                speeds[i] = Random.Range(speedRange.x, speedRange.y);
            }
        }

        private void Update()
        {
            if (stars == null || speeds == null)
            {
                return;
            }

            var delta = Time.deltaTime;
            for (var i = 0; i < stars.Length; i++)
            {
                var t = stars[i];
                var pos = t.position;
                pos.y -= speeds[i] * delta;
                if (pos.y < bottomY)
                {
                    pos.y = topY;
                    pos.x = Random.Range(minX, maxX);
                }
                t.position = pos;
            }
        }
    }
}
