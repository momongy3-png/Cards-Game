using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    //singletone property
    public static ScoreSystem Instance;

    [SerializeField] TMP_Text scoreText;
    int score = 0;

    [SerializeField]
    ParticleSystem correctEffect, wrongEffect;

    #region singletone initialize and update score on start
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //load saved score
        score = PlayerPrefs.GetInt("Score", 0);
        UpdateScoreText();
    }
    #endregion
    #region Score Setup
    public void AddScore()
    {
        score += 5;
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();
        UpdateScoreText();
    }
    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }
    #endregion
    #region VFXs play
    public void PlayEffect(int effectIndex)
    {
        //0 = wrong
        if (effectIndex == 0)
        {
            wrongEffect.Play();
        }
        //1 = correct
        else if (effectIndex == 1)
        {
            correctEffect.Play();
        }
    }

    #endregion
}
