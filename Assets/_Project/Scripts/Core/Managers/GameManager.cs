using UnityEngine;
using BlockBlast.Core;

namespace BlockBlast.Managers
{
    /// <summary>
    /// Quản lý điểm số, xu và kỷ lục chơi game
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int CurrentScore { get; private set; }
        public int HighScore { get; private set; }
        public int Coins { get; private set; }

        private const string HIGH_SCORE_KEY = "BlockBlast_HighScore";

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            HighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            Coins = 6527; // Điểm xu cố định ban đầu
        }

        private void OnEnable()
        {
            GameEvents.OnLinesCleared += HandleLinesCleared;
            GameEvents.OnGameOver += HandleGameOver;
        }

        private void OnDisable()
        {
            GameEvents.OnLinesCleared -= HandleLinesCleared;
            GameEvents.OnGameOver -= HandleGameOver;
        }

        private void Start()
        {
            GameEvents.TriggerScoreChanged(CurrentScore);
            GameEvents.TriggerHighScoreChanged(HighScore);
            GameEvents.TriggerCoinsChanged(Coins);
        }

        private void HandleLinesCleared(int lineCount)
        {
            int addedScore = lineCount * 100 * lineCount; // Thưởng combo
            int addedCoins = lineCount * 15;

            CurrentScore += addedScore;
            Coins += addedCoins;

            if (CurrentScore > HighScore)
            {
                HighScore = CurrentScore;
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, HighScore);
                GameEvents.TriggerHighScoreChanged(HighScore);
            }

            GameEvents.TriggerScoreChanged(CurrentScore);
            GameEvents.TriggerCoinsChanged(Coins);
        }

        private void HandleGameOver()
        {
            GameEvents.TriggerGameStateChanged(GameState.GameOver);
        }
    }
}