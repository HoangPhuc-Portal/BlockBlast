using System;
using UnityEngine;

namespace BlockBlast.Core
{
    /// <summary>
    /// Hệ thống Observer Pattern trung gian để truyền nhận các sự kiện toàn cục trong game
    /// </summary>
    public static class GameEvents
    {
        // Sự kiện Dữ liệu (Điểm số, Điểm cao nhất, Số xu)
        public static Action<int> OnScoreChanged;
        public static Action<int> OnHighScoreChanged;
        public static Action<int> OnCoinsChanged;

        // Sự kiện Trạng thái Game
        public static Action<GameState> OnGameStateChanged;

        // Sự kiện Gameplay
        public static Action<int> OnLinesCleared; // Số lượng hàng/cột bị xóa cùng lúc
        public static Action OnShapePlaced;
        public static Action OnGameOver;

        // Sự kiện Âm thanh
        public static Action OnPlayBlockPlaceSound;
        public static Action OnPlayLineClearSound;

        // Các hàm Trigger sự kiện
        public static void TriggerScoreChanged(int newScore) => OnScoreChanged?.Invoke(newScore);
        public static void TriggerHighScoreChanged(int highScore) => OnHighScoreChanged?.Invoke(highScore);
        public static void TriggerCoinsChanged(int newCoins) => OnCoinsChanged?.Invoke(newCoins);
        public static void TriggerGameStateChanged(GameState state) => OnGameStateChanged?.Invoke(state);
        public static void TriggerLinesCleared(int lineCount) => OnLinesCleared?.Invoke(lineCount);
        public static void TriggerShapePlaced() => OnShapePlaced?.Invoke();
        public static void TriggerGameOver() => OnGameOver?.Invoke();
        public static void TriggerPlayBlockPlaceSound() => OnPlayBlockPlaceSound?.Invoke();
        public static void TriggerPlayLineClearSound() => OnPlayLineClearSound?.Invoke();
    }

    public enum GameState
    {
        MainMenu,
        Gameplay,
        Paused,
        GameOver
    }
}