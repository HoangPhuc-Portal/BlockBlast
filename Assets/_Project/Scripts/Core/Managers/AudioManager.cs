using UnityEngine;
using BlockBlast.Core;

namespace BlockBlast.Managers
{
    /// <summary>
    /// Singleton quản lý các hiệu ứng âm thanh trong trò chơi
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource bgmSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip blockPlaceClip;
        [SerializeField] private AudioClip lineClearClip;
        [SerializeField] private AudioClip gameOverClip;

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

        private void OnEnable()
        {
            GameEvents.OnShapePlaced += PlayBlockPlaceSound;
            GameEvents.OnLinesCleared += PlayLineClearSound;
            GameEvents.OnGameOver += PlayGameOverSound;
        }

        private void OnDisable()
        {
            GameEvents.OnShapePlaced -= PlayBlockPlaceSound;
            GameEvents.OnLinesCleared -= PlayLineClearSound;
            GameEvents.OnGameOver -= PlayGameOverSound;
        }

        private void PlayBlockPlaceSound()
        {
            if (blockPlaceClip != null && sfxSource != null)
                sfxSource.PlayOneShot(blockPlaceClip);
        }

        private void PlayLineClearSound(int lineCount)
        {
            if (lineClearClip != null && sfxSource != null)
                sfxSource.PlayOneShot(lineClearClip, 1f + (lineCount * 0.1f));
        }

        private void PlayGameOverSound()
        {
            if (gameOverClip != null && sfxSource != null)
                sfxSource.PlayOneShot(gameOverClip);
        }
    }
}