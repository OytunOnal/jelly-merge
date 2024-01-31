using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon.Store
{
    public class UIStoreItemsGrid : MonoBehaviour
    {
        [SerializeField] GridLayoutGroup gridLayourGroup;

        private List<UIStoreItem> storeItemsList = new List<UIStoreItem>();
        public List<UIStoreItem> StoreItemsList => storeItemsList;

        private Pool storeItemPool;

        public void Init(List<ProductData> products, string selectedProductId)
        {
            UIStorePage uiStore = UIController.GetPage<UIStorePage>();

            storeItemPool = PoolManager.GetPoolByName(uiStore.STORE_ITEM_POOL_NAME);
            storeItemsList.Clear();

            gridLayourGroup.enabled = true;

            for (int i = 0; i < products.Count; i++)
            {
                UIStoreItem item = storeItemPool.GetPooledObject(new PooledObjectSettings().SetParrent(transform)).GetComponent<UIStoreItem>();
                storeItemsList.Add(item);

                item.transform.localScale = Vector3.one;
                item.transform.SetParent(transform);

                item.Init(products[i], products[i].UniqueId == selectedProductId);
            }

            var height = 280 * (products.Count / 3 + 1) + 40 * (products.Count / 3f);

            var rect = GetComponent<RectTransform>();

            rect.sizeDelta = rect.sizeDelta.SetY(height);

            Tween.DelayedCall(0.1f, () => gridLayourGroup.enabled = false);
        }

        public void UpdateItems(string selectedProductId)
        {
            for (int i = 0; i < storeItemsList.Count; i++)
            {
                storeItemsList[i].SetSelectedStatus(storeItemsList[i].Data.UniqueId == selectedProductId);
                storeItemsList[i].UpdatePriceText();
            }
        }
    }
}

