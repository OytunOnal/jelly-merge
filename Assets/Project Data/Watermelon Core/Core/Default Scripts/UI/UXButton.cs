using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JellyMerge
{
    public class UXButton : BaseButton
    {
        public UnityEvent onClick;

        public override void OnClick(Tween.TweenCallback callback = null)
        {
            base.OnClick(delegate
            {
                if (onClick != null)
                    onClick.Invoke();
            });
        }
    }
}