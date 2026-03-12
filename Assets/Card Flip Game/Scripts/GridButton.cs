using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridButton : MonoBehaviour
{
    [HideInInspector]
    public Animator anim;
    Button btn;

    static readonly int selectHash = Animator.StringToHash("select");
    static readonly int deselectHash = Animator.StringToHash("deselect");

    static List<GridButton> selectedButtons = new List<GridButton>();
    bool isSelected = false;
    static bool ableToSelect = true;
    [SerializeField]
    AudioClip correctClip, wrongClip;

    #region initialize
    void Awake()
    {
        anim = GetComponent<Animator>();
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnPressed);
    }

    void OnPressed()
    {
        if (!ableToSelect) return;

        if (isSelected)
        {
            anim.SetTrigger(deselectHash);
            selectedButtons.Remove(this);
            isSelected = false;
            return;
        }

        anim.SetTrigger(selectHash);
        selectedButtons.Add(this);
        isSelected = true;

        if (selectedButtons.Count == 2)
        {
            ableToSelect = false;

            Image img1 = selectedButtons[0].transform.GetChild(0).GetComponent<Image>();
            Image img2 = selectedButtons[1].transform.GetChild(0).GetComponent<Image>();

            if (img1.sprite == img2.sprite)
            {
                Invoke(nameof(CorrectCards), 1f);
            }
            else
            {
                Invoke(nameof(WrondCards), 1f);
            }
            
        }
    }
    #endregion
    #region Cards Validation
    void CorrectCards()
    {
        foreach (GridButton btn in selectedButtons)
        {
            btn.gameObject.SetActive(false);
            btn.isSelected = false;
        }
        ScoreSystem.Instance.AddScore();
        ScoreSystem.Instance.PlayEffect(1);
        SoundSystem.Instance.PlaySound(correctClip);
        selectedButtons.Clear();
        ableToSelect = true;
    }

    void WrondCards()
    {
        foreach (GridButton btn in selectedButtons)
        {
            btn.anim.SetTrigger(deselectHash);
            btn.isSelected = false;
        }
        ScoreSystem.Instance.PlayEffect(0);
        SoundSystem.Instance.PlaySound(wrongClip);
        selectedButtons.Clear();
        ableToSelect = true;
    }
    #endregion
}
