using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon.TrainMatch
{
    public class LivesManager : MonoBehaviour
    {
        private static LivesManager instance;

        [SerializeField] LivesData data;

        [Space]
        [SerializeField] TMP_Text livesCountText;
        [SerializeField] TMP_Text durationText;

        [Space] 
        [SerializeField] Button addButton;

        [Space]
        [SerializeField] AddLivesPanel addLivesPanel;

        private static LivesSave livesSave;

        public static int Lives { get => livesSave.livesCount; private set => SetLifes(value); }
        private static DateTime LivesDate { get => livesSave.date; set => livesSave.date = value; }

        private static Coroutine livesCoroutine;


        private void Awake()
        {
            instance = this;

            livesSave = SaveController.GetSaveObject<LivesSave>("Lives");
            livesSave.Init(data);

            // For init purposses
            SetLifes(Lives);

            if (Lives < data.maxLivesCount)
            {
                livesCoroutine = Tween.InvokeCoroutine(LivesCoroutine());
            }
            else
            {
                durationText.text = data.fullText;
                addLivesPanel.SetTime(data.fullText);
            }

            addButton.onClick.AddListener(LifeForAd);

            addLivesPanel.Init();
        }

        private static void SetLifes(int value)
        {
            livesSave.livesCount = value;
            instance.livesCountText.text = Lives.ToString();

            instance.addButton.gameObject.SetActive(Lives != instance.data.maxLivesCount);

            instance.addLivesPanel.SetLivesCount(value);
        }

        [Button]
        public void RemoveLifeDev()
        {
            RemoveLife();
        }

        public static void RemoveLife()
        {
            Lives--;
            if (Lives < 0) Lives = 0;

            if (livesCoroutine == null)
            {
                LivesDate = DateTime.Now;
                livesCoroutine = Tween.InvokeCoroutine(instance.LivesCoroutine());
            }
        }

        public static void AddLife()
        {
            Lives++;
        }

        private  IEnumerator LivesCoroutine()
        {
            var oneLifeSpan = TimeSpan.FromSeconds(data.oneLifeRestorationDuration);

            var wait = new WaitForSeconds(0.25f);
            while(Lives < data.maxLivesCount)
            {
                var timespan = DateTime.Now - LivesDate;

                if(timespan >= oneLifeSpan)
                {
                    Lives++;

                    LivesDate = DateTime.Now;
                }

                durationText.text = string.Format(data.timespanFormat, oneLifeSpan - timespan);
                addLivesPanel.SetTime(durationText.text);

                yield return wait;
            }

            durationText.text = data.fullText;
            addLivesPanel.SetTime(data.fullText);

            livesCoroutine = null;
        }

        private void LifeForAd()
        {
            addLivesPanel.Show();
        }

        private class LivesSave : ISaveObject
        {
            public int livesCount;

            public long dateBinary;
            public DateTime date;

            [SerializeField] bool firstTime = true;

            public void Init(LivesData data)
            {
                if (firstTime)
                {
                    firstTime = false;

                    livesCount = data.maxLivesCount;
                    date = DateTime.Now;
                } else
                {
                    date = DateTime.FromBinary(dateBinary);

                    if(livesCount < data.maxLivesCount)
                    {
                        var timeDif = DateTime.Now - date;

                        var oneLifeSpan = TimeSpan.FromSeconds(data.oneLifeRestorationDuration);

                        while (timeDif >= oneLifeSpan && livesCount < data.maxLivesCount)
                        {
                            timeDif -= oneLifeSpan;
                            date += oneLifeSpan;

                            livesCount++;
                        }
                    }
                }
            }

            public void Flush()
            {
                dateBinary = date.ToBinary();
            }
        }
    }

    
}