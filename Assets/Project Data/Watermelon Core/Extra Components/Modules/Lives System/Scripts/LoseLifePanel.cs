using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;
using Watermelon.TrainMatch;

public class LoseLifePanel : MonoBehaviour
{
    [SerializeField] RectTransform panel;
    [SerializeField] Vector3 hidePos;
    private Vector3 showPos;

    [SerializeField] Image backgroundImage;

    [SerializeField] Button button;
    [SerializeField] Button closeButton;

    Color backColor;

    public void Init()
    {
        backColor = backgroundImage.color;
        showPos = panel.anchoredPosition;

        button.onClick.AddListener(OnButtonClick);
        closeButton.onClick.AddListener(Hide);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        backgroundImage.color = Color.clear;
        backgroundImage.DOColor(backColor, 0.3f);

        panel.anchoredPosition = hidePos;
        panel.DOAnchoredPosition(showPos, 0.3f).SetEasing(Ease.Type.SineOut);

    }

    public void Hide()
    {
        backgroundImage.DOColor(Color.clear, 0.3f);
        panel.DOAnchoredPosition(hidePos, 0.3f).SetEasing(Ease.Type.SineIn).OnComplete(() => gameObject.SetActive(false));
    }

    public void OnButtonClick()
    {
        LivesManager.RemoveLife();
        Hide();
        UIController.HidePage<UIGame>();
        UIController.ShowPage<UIMainMenu>();
    }
}
