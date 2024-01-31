﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Watermelon
{
    [System.Serializable]
    public class FloatingCloud
    {
        private static FloatingCloud floatingCloud;

        [SerializeField] Case[] floatingCloudCases;

        [Space]
        [SerializeField] TextMeshProUGUI floatingText;

        private static Dictionary<int, Case> floatingCloudCasesLink = new Dictionary<int, Case>();

        public void Initialise()
        {
            floatingCloud = this;

            for (int i = 0; i < floatingCloudCases.Length; i++)
            {
                RegisterCase(floatingCloudCases[i]);
            }
        }

        public static void RegisterCase(FloatingCloudSettings floatingCloudSettings)
        {
            int cloudHash = StringToHash(floatingCloudSettings.Name);

            if (floatingCloudCasesLink.ContainsKey(cloudHash))
            {
                Debug.LogError($"Cloud {floatingCloudSettings.Name} already registered!");

                return;
            }

            Case floatingCloudCase = new Case(floatingCloudSettings);
            floatingCloudCase.Initialise();

            floatingCloudCasesLink.Add(cloudHash, floatingCloudCase);
        }

        public static void RegisterCase(Case floatingCloudCase)
        {
            int cloudHash = StringToHash(floatingCloudCase.Name);

            if (floatingCloudCasesLink.ContainsKey(cloudHash))
            {
                Debug.LogError($"Cloud {floatingCloudCase.Name} already registered!");

                return;
            }

            floatingCloudCase.Initialise();

            floatingCloudCasesLink.Add(cloudHash, floatingCloudCase);
        }

        public static void SpawnCurrency(string key, RectTransform rectTransform, RectTransform targetTransform, int elementsAmount, string text, System.Action onCurrencyHittedTarget = null)
        {
            SpawnCurrency(key.GetHashCode(), rectTransform, targetTransform, elementsAmount, text, onCurrencyHittedTarget);
        }

        public static void SpawnCurrency(int hash, RectTransform rectTransform, RectTransform targetTransform, int elementsAmount, string text, System.Action onCurrencyHittedTarget = null)
        {
            if (!floatingCloudCasesLink.ContainsKey(hash))
            {
                Debug.LogError($"Cloud with hash {hash} isn't registered!");

                return;
            }

            Case floatingCloudCase = floatingCloudCasesLink[hash];

            RectTransform targetRectTransform = targetTransform;

            floatingCloudCase.Pool.ReturnToPoolEverything(true);

            // Play appear sound
            if (floatingCloudCase.AppearAudioClip != null)
                AudioController.PlaySound(floatingCloudCase.AppearAudioClip);

            float cloudRadius = floatingCloudCase.CloudRadius;
            Vector3 centerPoint = rectTransform.position;

            float defaultPitch = 0.9f;
            bool currencyHittedTarget = false;
            for (int i = 0; i < elementsAmount; i++)
            {
                GameObject elementObject = floatingCloudCase.Pool.GetPooledObject();
                elementObject.transform.SetParent(targetRectTransform);

                //elementObject.transform.SetParent(targetRectTransform);
                //elementObject.transform.SetAsLastSibling();
                //elementObject.transform.SetParent(currencyCloud.coinsContainerRectTransform);

                elementObject.transform.position = centerPoint;
                elementObject.transform.localRotation = Quaternion.identity;
                elementObject.transform.localScale = Vector3.one;

                Image elementImage = elementObject.GetComponent<Image>();
                elementImage.color = Color.white.SetAlpha(0);

                float moveTime = Random.Range(0.6f, 0.8f);

                TweenCase currencyTweenCase = null;
                RectTransform elementRectTransform = (RectTransform)elementObject.transform;

                elementImage.DOFade(1, 0.2f, unscaledTime: true);
                elementRectTransform.DOAnchoredPosition(elementRectTransform.anchoredPosition + (Random.insideUnitCircle * cloudRadius), moveTime, unscaledTime: true).SetEasing(Ease.Type.CubicOut).OnComplete(delegate
                {
                    Tween.DelayedCall(0.1f, delegate
                    {
                        elementRectTransform.DOScale(0.3f, 0.5f, unscaledTime: true).SetEasing(Ease.Type.ExpoIn);
                        elementRectTransform.DOLocalMove(Vector3.zero, 0.5f, unscaledTime: true).SetEasing(Ease.Type.SineIn).OnComplete(delegate
                        {
                            if (!currencyHittedTarget)
                            {
                                if (onCurrencyHittedTarget != null)
                                    onCurrencyHittedTarget.Invoke();

                                currencyHittedTarget = true;
                            }

                            bool punchTarget = true;
                            if (currencyTweenCase != null)
                            {
                                if (currencyTweenCase.state < 0.8f)
                                {
                                    punchTarget = false;
                                }
                                else
                                {
                                    currencyTweenCase.Kill();
                                }
                            }

                            if (punchTarget)
                            {
                                // Play collect sound
                                if (floatingCloudCase.CollectAudioClip != null)
                                    AudioController.PlaySound(floatingCloudCase.CollectAudioClip, pitch: defaultPitch);

                                defaultPitch += 0.01f;

                                currencyTweenCase = targetRectTransform.DOScale(1.2f, 0.15f, unscaledTime: true).OnComplete(delegate
                                {
                                    currencyTweenCase = targetRectTransform.DOScale(1.0f, 0.1f, unscaledTime: true);
                                });
                            }

                            elementObject.transform.SetParent(targetRectTransform);
                            elementRectTransform.gameObject.SetActive(false);
                        });
                    }, unscaledTime: true);
                });
            }

            if (!string.IsNullOrEmpty(text))
            {
                floatingCloud.floatingText.gameObject.SetActive(true);
                floatingCloud.floatingText.text = text;
                floatingCloud.floatingText.transform.localScale = Vector3.zero;
                floatingCloud.floatingText.transform.SetParent(targetRectTransform);
                floatingCloud.floatingText.transform.SetAsLastSibling();
                floatingCloud.floatingText.transform.position = rectTransform.position;
                floatingCloud.floatingText.color = floatingCloud.floatingText.color.SetAlpha(1.0f);
                floatingCloud.floatingText.transform.DOScale(1, 0.3f, unscaledTime: true).SetEasing(Ease.Type.CubicOut).OnComplete(delegate
                {
                    floatingCloud.floatingText.DOFade(0, 0.5f, unscaledTime: true).SetEasing(Ease.Type.ExpoIn);
                    floatingCloud.floatingText.transform.DOMove(floatingCloud.floatingText.transform.position.AddToY(0.1f), 0.5f, unscaledTime: true).SetEasing(Ease.Type.QuadIn).OnComplete(delegate
                    {
                        floatingCloud.floatingText.gameObject.SetActive(false);
                    });
                });
            }
        }

        public static void FloatingText(string text, RectTransform targetRectTransform, Vector3 position, int fontSize = 130)
        {
            if (!string.IsNullOrEmpty(text))
            {
                floatingCloud.floatingText.gameObject.SetActive(true);
                floatingCloud.floatingText.text = text;
                floatingCloud.floatingText.fontSize = fontSize;
                floatingCloud.floatingText.transform.localScale = Vector3.zero;
                floatingCloud.floatingText.transform.SetParent(targetRectTransform);
                floatingCloud.floatingText.transform.SetAsLastSibling();
                floatingCloud.floatingText.transform.position = position;
                floatingCloud.floatingText.color = floatingCloud.floatingText.color.SetAlpha(1.0f);
                floatingCloud.floatingText.transform.DOScale(1, 0.3f, unscaledTime: true).SetEasing(Ease.Type.CubicOut).OnComplete(delegate
                {
                    floatingCloud.floatingText.DOFade(0, 1.2f, unscaledTime: true).SetEasing(Ease.Type.ExpoIn);
                    floatingCloud.floatingText.transform.DOMove(floatingCloud.floatingText.transform.position.AddToY(0.1f), 1.2f, unscaledTime: true).SetEasing(Ease.Type.QuadIn).OnComplete(delegate
                    {
                        floatingCloud.floatingText.gameObject.SetActive(false);
                    });
                });
            }
        }

        public static int StringToHash(string cloudName)
        {
            return cloudName.GetHashCode();
        }

        [System.Serializable]
        public class Case
        {
            [SerializeField] string name;
            public string Name => name;

            [SerializeField] GameObject prefab;
            public GameObject Prefab => prefab;

            [SerializeField] AudioClip appearAudioClip;
            public AudioClip AppearAudioClip => appearAudioClip;

            [SerializeField] AudioClip collectAudioClip;
            public AudioClip CollectAudioClip => collectAudioClip;

            [Space]
            [SerializeField] float cloudRadius;
            public float CloudRadius => cloudRadius;

            public Pool Pool { private set; get; }

            public Case(FloatingCloudSettings settings)
            {
                name = settings.Name;
                prefab = settings.Prefab;

                cloudRadius = settings.CloudRadius;

                appearAudioClip = settings.AppearAudioClip;
                collectAudioClip = settings.CollectAudioClip;
            }

            public void Initialise()
            {
                Pool = new Pool(new PoolSettings("FloatingCloud_" + name, prefab, 10, true));
            }
        }
    }
}
