using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using JellyMerge;

namespace JellyMerge
{
    public class UIComplete : UIPage
    {
        [SerializeField] TMP_Text levelCompleteText;

        [Space(5)]
        [SerializeField] SpriteRenderer[] levelCompleteCubeSprRends;
        [SerializeField] ParticleSystem[] confettiParticles;

        private static UIComplete instance;

        public override void Initialise()
        {
            instance = this;
        }

        #region Show/Hide
        public override void PlayShowAnimation()
        {
            if (isPageDisplayed)
                return;

            gameObject.SetActive(true);
            levelCompleteText.text = "LEVEL " + (GameController.CurrentLevelIndex + 1);

            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            if (!isPageDisplayed)
                return;

            gameObject.SetActive(false);

            UIController.OnPageClosed(this);
        }


        public static void InitUIColors(ColorsPreset preset)
        {
            for (int i = 0; i < instance.levelCompleteCubeSprRends.Length; i++)
            {
                instance.levelCompleteCubeSprRends[i].color = preset.uiDarker;
            }

            for (int i = 0; i < instance.confettiParticles.Length; i++)
            {
                ParticleSystem.MainModule main = instance.confettiParticles[i].main;
                main.startColor = new ParticleSystem.MinMaxGradient(preset.uiDarker, preset.uiLighter);
            }
        }

        #endregion
    }
}
