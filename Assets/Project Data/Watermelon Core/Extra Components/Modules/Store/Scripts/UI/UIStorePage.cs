using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Watermelon.Store
{
    public class UIStorePage : UIPage
    {
        private readonly float PANEL_BOTTOM_OFFSET_Y = -2000f;
        public readonly string STORE_ITEM_POOL_NAME = "StoreItem";

        [SerializeField] List<UIStoreTab> tabs;
        private Dictionary<TabData, UIStoreTab> tabsDictionary;

        [Space]
        [SerializeField] RectTransform storeAnimatedPanelRect;
        [SerializeField] Image storePanelBackground;
        [SerializeField] ScrollRect productsScroll;
        [SerializeField] GameObject scrollbarVertical;
        [SerializeField] Image scrollFadeImage;

        [Header("Prefabs")]
        [SerializeField] GameObject storeItemPrefab;

        [Header("Preview")]
        [SerializeField] CanvasGroup previewCanvasGroup;
        [SerializeField] Image backgroundImage;
        [SerializeField] Image previewImage;
        [SerializeField] RawImage previewRawImage;

        [Space]
        [SerializeField] Button closeButton;
        [SerializeField] RectTransform coinsPanel;
        [SerializeField] TextMeshProUGUI coinsText;
        [SerializeField] Image coinsPanelCurrencyImage;
        [SerializeField] TextMeshProUGUI coinsForAdsText;
        [SerializeField] Image coinsForAdsCurrencyImage;

        [Space]
        [SerializeField] UIStoreItemsGrid storeGrid;
        [SerializeField] ScrollRect scrollView;

        private static PoolGeneric<UIStoreItem> storeItemPool;
        private static float startedStorePanelRectPositionY;

        private ProductData SelectedProductData { get; set; }

        private List<ProductData> currentPageProducts;
        private List<ProductData> lockedPageProducts = new List<ProductData>();

        private Currency rewardForAdsCurrency;

        public override void Initialise()
        {
            startedStorePanelRectPositionY = storeAnimatedPanelRect.anchoredPosition.y;

            storeItemPool = PoolManager.AddPool<UIStoreItem>(new PoolSettings(STORE_ITEM_POOL_NAME, storeItemPrefab, 10, true));

            coinsForAdsText.text = "GET\n" + StoreController.CoinsForAdsAmount;

            rewardForAdsCurrency = CurrenciesController.GetCurrency(StoreController.CoinsForAdsCurrency);
        }

        public override void PlayShowAnimation()
        {
            previewCanvasGroup.alpha = 0;
            previewCanvasGroup.DOFade(1, 0.3f);

            InitStoreUI();

            storeAnimatedPanelRect.anchoredPosition = storeAnimatedPanelRect.anchoredPosition.SetY(PANEL_BOTTOM_OFFSET_Y);

            storeAnimatedPanelRect.DOAnchoredPosition(new Vector3(storeAnimatedPanelRect.anchoredPosition.x, startedStorePanelRectPositionY + 100f, 0f), 0.4f).SetEasing(Ease.Type.SineInOut).OnComplete(delegate
            {
                storeAnimatedPanelRect.DOAnchoredPosition(new Vector3(storeAnimatedPanelRect.anchoredPosition.x, startedStorePanelRectPositionY, 0f), 0.2f).SetEasing(Ease.Type.SineInOut).OnComplete(() =>
                {
                    UIController.OnPageOpened(this);
                });
            });

            closeButton.transform.localScale = Vector3.zero;
            coinsPanel.localScale = Vector3.zero;

            closeButton.DOScale(1, 0.3f).SetEasing(Ease.Type.SineOut);
            coinsPanel.DOScale(1, 0.3f).SetEasing(Ease.Type.SineOut);

            rewardForAdsCurrency.OnCurrencyChanged += OnCurrencyAmountChanged;

            coinsText.text = rewardForAdsCurrency.AmountFormatted;
            coinsPanelCurrencyImage.sprite = rewardForAdsCurrency.Icon;
            coinsForAdsCurrencyImage.sprite = rewardForAdsCurrency.Icon;
        }

        private StorePreview3D storePreview3D;

        public void InitStoreUI()
        {
            // Clear pools
            storeItemPool?.ReturnToPoolEverything(true);

            SelectedProductData = StoreController.GetSelectedProductData(StoreController.SelectedTabData);

            var tab = StoreController.SelectedTabData;
            if (tab.PreviewType == PreviewType.Preview_2D)
            {
                previewRawImage.enabled = false;
                backgroundImage.enabled = true;
                previewImage.enabled = true;
            }
            else
            {
                previewRawImage.enabled = true;
                backgroundImage.enabled = false;
                previewImage.enabled = false;

                if (storePreview3D != null)
                    Destroy(storePreview3D.gameObject);

                storePreview3D = Instantiate(tab.PreviewPrefab).GetComponent<StorePreview3D>();
                storePreview3D.Init();
                storePreview3D.SpawnProduct(SelectedProductData);

                previewRawImage.texture = storePreview3D.Texture;
            }

            previewImage.sprite = SelectedProductData.Preview2DSprite;
            backgroundImage.color = StoreController.SelectedTabData.BackgroundColor;

            InitPage();

            UpdateCurrentPage(true);

            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].SetSelectedStatus(tabs[i].Data == StoreController.SelectedTabData);
            }
        }

        private void InitPage()
        {
            storeGrid.Init(StoreController.GetProducts(StoreController.SelectedTabData), SelectedProductData.UniqueId);

            scrollView.normalizedPosition = Vector2.up;
        }

       

        private void UpdateCurrentPage(bool redrawStorePage)
        {
            currentPageProducts = StoreController.GetProducts(StoreController.SelectedTabData);

            lockedPageProducts.Clear();

            for (int i = 0; i < currentPageProducts.Count; i++)
            {
                var product = currentPageProducts[i];

                if (!product.IsUnlocked && !product.IsDummy)
                {
                    lockedPageProducts.Add(product);
                }
            }

            if (redrawStorePage)
            {
                storeGrid.UpdateItems(SelectedProductData.UniqueId);
            }

            storePanelBackground.color = StoreController.SelectedTabData.BackgroundColor;
            scrollFadeImage.color = StoreController.SelectedTabData.BackgroundColor;

            productsScroll.enabled = currentPageProducts.Count > 6;
            scrollbarVertical.SetActive(currentPageProducts.Count > 6);
            scrollFadeImage.gameObject.SetActive(currentPageProducts.Count > 6);
        }

        public override void PlayHideAnimation()
        {
            rewardForAdsCurrency.OnCurrencyChanged -= OnCurrencyAmountChanged;

            closeButton.DOScale(0, 0.3f).SetEasing(Ease.Type.SineIn);
            coinsPanel.DOScale(0, 0.3f).SetEasing(Ease.Type.SineIn);

            if (storePreview3D != null)
            {
                Destroy(storePreview3D.gameObject);
            }

            previewCanvasGroup.DOFade(0, 0.3f);

            storeAnimatedPanelRect.DOAnchoredPosition(new Vector3(storeAnimatedPanelRect.anchoredPosition.x, startedStorePanelRectPositionY + 100f, 0f), 0.2f).SetEasing(Ease.Type.SineInOut).OnComplete(delegate
            {
                storeAnimatedPanelRect.DOAnchoredPosition(new Vector3(storeAnimatedPanelRect.anchoredPosition.x, PANEL_BOTTOM_OFFSET_Y, 0f), 0.4f).SetEasing(Ease.Type.SineInOut).OnComplete(delegate
                {
                    UIController.OnPageClosed(this);
                });
            });
        }

        private void OnCurrencyAmountChanged(Currency currency, int difference)
        {
            coinsText.text = currency.AmountFormatted;
        }

        public void InitTabs(TabData.SimpleTabDelegate OnTabClicked)
        {
            tabsDictionary = new Dictionary<TabData, UIStoreTab>();

            if (StoreController.TabsCount > tabs.Count)
                Debug.LogError("[Store Module] Not enough tabs in store page ui");

            var tabsCount = Mathf.Min(StoreController.TabsCount, tabs.Count);

            for (int i = 0; i < tabsCount; i++)
            {
                var tab = tabs[i];
                var tabData = StoreController.GetTab(i);

                tab.gameObject.SetActive(true);
                tab.Init(tabData, OnTabClicked);

                tabsDictionary.Add(tabData, tab);
            }

            for (int i = tabsCount; i < tabs.Count; i++)
            {
                tabs[i].gameObject.SetActive(false);
            }
        }

        public void SetSelectedTab(TabData tabData)
        {
            foreach (var tab in tabs)
            {
                tab.SetSelectedStatus(tab.Data == tabData);
            }

            InitStoreUI();
        }

        public void GetCoinsForAdsButton()
        {
            AdsManager.ShowRewardBasedVideo((bool success) =>
            {
                if (success)
                {
                    FloatingCloud.SpawnCurrency("Coins", coinsForAdsText.rectTransform, coinsText.rectTransform, 20, "", () =>
                    {
                        CurrenciesController.Add(Currency.Type.Coins, StoreController.CoinsForAdsAmount);

                        UpdateCurrentPage(true);
                    });
                }
            });
        }

        public void CloseButton()
        {
            UIController.HidePage<UIStorePage>(() =>
            {
                UIController.ShowPage<UIMainMenu>();
            });
        }

        public static void EnablePreviewRawImage()
        {
            UIController.GetPage<UIStorePage>().previewRawImage.enabled = true;
        }

        public static void DisablePreviewRawImage()
        {
            UIController.GetPage<UIStorePage>().previewRawImage.enabled = false;
        }
    }
}