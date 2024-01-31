using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Watermelon.JellyMerge;

namespace Watermelon
{
    public class UIGame : UIPage
    {
        [SerializeField] TMP_Text levelText;
        [SerializeField] Button skipLevelButton;
        [SerializeField] Button restartButton;

        [Space]
        [SerializeField] GameObject tutorialPanel;
        [SerializeField] GameObject horizontalTutorialPanel;
        [SerializeField] GameObject verticalTutorialPanel;

        public override void Initialise()
        {
            restartButton.onClick.AddListener(RestartButton);
            skipLevelButton.onClick.AddListener(SkipLevelButton);
        }

        private void OnRewardedAdLoaded()
        {
            skipLevelButton.gameObject.SetActive(true);
        }

        public void ShowTutorialPanel(bool horizontal)
        {
            tutorialPanel.SetActive(true);

            if (horizontal)
            {
                horizontalTutorialPanel.SetActive(true);
                verticalTutorialPanel.SetActive(false);
            }
            else
            {
                horizontalTutorialPanel.SetActive(false);
                verticalTutorialPanel.SetActive(true);
            }
        }

        public void HideTutorialPanel()
        {
            tutorialPanel.SetActive(false);
            verticalTutorialPanel.SetActive(false);
            horizontalTutorialPanel.SetActive(false);
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            levelText.text = "LEVEL " + (GameController.CurrentLevelIndex + 1);

            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            UIController.OnPageClosed(this);
        }

        #endregion

        #region Buttons

        public void RestartButton()
        {
            GameController.Restart();

            AudioController.PlaySound(AudioController.Sounds.buttonSound);
        }

        public void SkipLevelButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

        }

        #endregion
    }
}
