using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingScene : MonoBehaviour
{
    public Image bar;
    private void Start()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(bar.DOFillAmount(0.3f, 0.5f));
        sequence.Append(bar.DOFillAmount(0.6f, 0.5f));
        sequence.Append(bar.DOFillAmount(0.8f, 0.5f));
        sequence.Append(bar.DOFillAmount(1f, 0.5f));
        sequence.OnComplete(delegate
        {
            Initiate.Fade("GamePlayScene", Color.black, 2f);
        });
    }
}
