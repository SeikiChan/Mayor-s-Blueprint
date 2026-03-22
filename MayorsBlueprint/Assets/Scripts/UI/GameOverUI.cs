using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MayorsBlueprint.Core;
using MayorsBlueprint.Events;

namespace MayorsBlueprint.UI
{
    /// <summary>
    /// Win/Lose screen shown at the end of a run.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private Button restartButton;

        private void Start()
        {
            if (panel != null)
                panel.SetActive(false);

            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
        }

        private void OnEnable()
        {
            GameEvents.OnRunEnded += ShowResult;
        }

        private void OnDisable()
        {
            GameEvents.OnRunEnded -= ShowResult;
        }

        private void ShowResult(bool won)
        {
            if (panel != null)
                panel.SetActive(true);

            if (resultText != null)
                resultText.text = won ? "Victory!" : "Defeat...";

            var resources = FindAnyObjectByType<ResourceManager>();
            if (finalScoreText != null && resources != null)
            {
                var config = GameManager.Instance?.Config;
                int target = config != null ? config.targetScore : 0;
                finalScoreText.text = $"Final Score: {resources.Score} / {target}";
            }
        }

        private void OnRestartClicked()
        {
            if (panel != null)
                panel.SetActive(false);

            GameManager.Instance?.RestartRun();
        }
    }
}
