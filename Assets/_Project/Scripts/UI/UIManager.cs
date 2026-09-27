using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using BlockBlast.Core;

namespace BlockBlast.UI
{
    /// <summary>
    /// Điều khiển toàn bộ giao diện người dùng UI (Thanh điểm, Xu, Bảng Game Over)
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Top Bar UI Elements")]
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;

        [Header("Game Over Modal")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;

        private void OnEnable()
        {
            GameEvents.OnScoreChanged += UpdateScore;
            GameEvents.OnHighScoreChanged += UpdateHighScore;
            GameEvents.OnCoinsChanged += UpdateCoins;
            GameEvents.OnGameOver += ShowGameOverModal;
        }

        private void OnDisable()
        {
            GameEvents.OnScoreChanged -= UpdateScore;
            GameEvents.OnHighScoreChanged -= UpdateHighScore;
            GameEvents.OnCoinsChanged -= UpdateCoins;
            GameEvents.OnGameOver -= ShowGameOverModal;
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null) scoreText.text = score.ToString();
        }

        private void UpdateHighScore(int highScore)
        {
            if (highScoreText != null) highScoreText.text = highScore.ToString();
        }

        private void UpdateCoins(int coins)
        {
            if (coinText != null) coinText.text = coins.ToString("N0");
        }

        private void ShowGameOverModal()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                if (finalScoreText != null && scoreText != null)
                {
                    finalScoreText.text = scoreText.text;
                }
            }
        }

        public void OnClickRestartButton()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}