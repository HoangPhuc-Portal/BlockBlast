using UnityEngine;
using BlockBlast.Core;

namespace BlockBlast.Core.StateMachine
{
    /// <summary>
    /// Quản lý vòng đời và trạng thái hoạt động của trò chơi
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        public GameState CurrentState { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            ChangeState(GameState.Gameplay);
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            GameEvents.TriggerGameStateChanged(newState);

            switch (newState)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;
                case GameState.Gameplay:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.GameOver:
                    Time.timeScale = 1f;
                    break;
            }
        }
    }
}