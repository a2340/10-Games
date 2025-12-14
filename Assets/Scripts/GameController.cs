using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceDodge
{
    public class GameController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerShip player;
        [SerializeField] private Asteroid asteroidPrefab;
        [SerializeField] private Transform asteroidParent;
        [SerializeField] private StarField starField;

        [Header("UI")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text bestText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text livesText;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private GameObject levelFlashPanel;
        [SerializeField] private TMP_Text levelFlashText;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button restartButton;

        [Header("Asteroid Settings")]
        [SerializeField] private Vector2 spawnRadiusRange = new Vector2(0.18f, 0.32f);
        [SerializeField] private float baseSpawnDelay = 1.2f;
        [SerializeField] private float spawnDelayFloor = 0.35f;
        [SerializeField] private float spawnDelayStep = 0.08f;
        [SerializeField] private float baseFallSpeed = 2.2f;
        [SerializeField] private float fallSpeedStep = 0.35f;
        [SerializeField] private float sideSpeedRange = 0.8f;

        [Header("Gameplay")]
        [SerializeField] private int startingLives = 3;
        [SerializeField] private float sidePadding = 0.6f;

        private readonly List<Asteroid> asteroids = new List<Asteroid>();

        private int score;
        private int best;
        private int level = 1;
        private int lives;
        private float spawnTimer;
        private float levelFlashTimer;
        private bool started;
        private bool gameOver;

        public bool IsRunning => started && !gameOver;

        private void Awake()
        {
            best = PlayerPrefs.GetInt("space_dodge_best", 0);
            restartButton?.onClick.AddListener(RestartRun);
        }

        private void Start()
        {
            ResetRun();
        }

        private void Update()
        {
            if (!started && Input.GetKeyDown(KeyCode.Space))
            {
                StartRun();
            }

            if (!IsRunning)
            {
                return;
            }

            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnAsteroid();
            }

            if (levelFlashTimer > 0f)
            {
                levelFlashTimer -= Time.deltaTime;
                if (levelFlashPanel != null)
                {
                    levelFlashPanel.SetActive(true);
                }
            }
            else if (levelFlashPanel != null)
            {
                levelFlashPanel.SetActive(false);
            }
        }

        private void ResetRun()
        {
            score = 0;
            level = 1;
            lives = startingLives;
            started = false;
            gameOver = false;
            spawnTimer = baseSpawnDelay;
            levelFlashTimer = 0f;

            ClearAsteroids();
            UpdateUI();
            ConfigureBounds();
            player?.ResetShip(GetPlayerStart());

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }

            if (hintText != null)
            {
                hintText.text = "Space ile başla · A/D veya Oklar";
            }
        }

        private void StartRun()
        {
            started = true;
            gameOver = false;
            spawnTimer = 0.05f; // hızlı başlangıç
            levelFlashTimer = 0f;

            if (hintText != null)
            {
                hintText.text = string.Empty;
            }
        }

        private void ConfigureBounds()
        {
            var cam = Camera.main;
            var halfHeight = cam.orthographicSize;
            var halfWidth = halfHeight * cam.aspect;
            var minX = -halfWidth + sidePadding;
            var maxX = halfWidth - sidePadding;
            player?.SetBounds(minX, maxX);

            if (starField != null)
            {
                starField.Configure(cam, sidePadding);
            }
        }

        private Vector2 GetPlayerStart()
        {
            var cam = Camera.main;
            var y = cam.orthographicSize - 1.6f;
            return new Vector2(0f, -y);
        }

        private void SpawnAsteroid()
        {
            if (asteroidPrefab == null || player == null)
            {
                return;
            }

            var cam = Camera.main;
            var halfHeight = cam.orthographicSize;
            var halfWidth = halfHeight * cam.aspect;
            var minX = -halfWidth + sidePadding;
            var maxX = halfWidth - sidePadding;

            var radius = Random.Range(spawnRadiusRange.x, spawnRadiusRange.y);
            var x = Random.Range(minX, maxX);
            var y = halfHeight + radius + 0.5f;
            var pos = new Vector3(x, y, 0f);

            var difficulty = 1f + (level - 1) * 0.3f;
            var fallSpeed = baseFallSpeed + difficulty * fallSpeedStep + Random.Range(0f, 0.9f);
            var sideSpeed = Random.Range(-sideSpeedRange, sideSpeedRange) * difficulty;

            var asteroid = Instantiate(asteroidPrefab, pos, Quaternion.identity, asteroidParent);
            var tint = new Color(Random.Range(0.6f, 1f), Random.Range(0.4f, 1f), Random.Range(0.6f, 1f));
            asteroid.Setup(this, fallSpeed, sideSpeed, radius, tint, minX, maxX, -halfHeight - 1f);
            asteroids.Add(asteroid);

            var spawnDelay = Mathf.Max(baseSpawnDelay - (level - 1) * spawnDelayStep, spawnDelayFloor);
            spawnTimer = spawnDelay;
        }

        public void HandleAsteroidPassed(Asteroid asteroid)
        {
            if (!IsRunning)
            {
                return;
            }

            score++;
            if (score > best)
            {
                best = score;
                PlayerPrefs.SetInt("space_dodge_best", best);
            }

            UpdateLevel();
            UpdateUI();
            RemoveAsteroid(asteroid);
        }

        public void HandlePlayerHit(Asteroid asteroid)
        {
            if (!IsRunning)
            {
                return;
            }

            lives--;
            player?.TriggerHitFeedback();
            RemoveAsteroid(asteroid);
            UpdateUI();

            if (lives <= 0)
            {
                EndRun();
            }
        }

        private void EndRun()
        {
            gameOver = true;
            started = false;
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }

        private void UpdateLevel()
        {
            var newLevel = 1 + score / 10;
            if (newLevel > level)
            {
                level = newLevel;
                levelFlashTimer = 1.2f;
                if (levelFlashText != null)
                {
                    levelFlashText.text = $"SEVİYE {level}";
                }
            }
        }

        private void UpdateUI()
        {
            if (scoreText != null) scoreText.text = score.ToString();
            if (bestText != null) bestText.text = best.ToString();
            if (levelText != null) levelText.text = level.ToString();
            if (livesText != null) livesText.text = lives.ToString();
        }

        private void RemoveAsteroid(Asteroid asteroid)
        {
            if (asteroid != null)
            {
                asteroids.Remove(asteroid);
                Destroy(asteroid.gameObject);
            }
        }

        private void ClearAsteroids()
        {
            foreach (var a in asteroids)
            {
                if (a != null)
                {
                    Destroy(a.gameObject);
                }
            }
            asteroids.Clear();
        }

        private void RestartRun()
        {
            ResetRun();
        }
    }
}
