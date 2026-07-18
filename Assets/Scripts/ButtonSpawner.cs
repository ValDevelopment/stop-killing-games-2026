using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ButtonSpawner : MonoBehaviour
{
    public static readonly KeyCode[] Keys =
    {
        KeyCode.W,
        KeyCode.A,
        KeyCode.S,
        KeyCode.D
    };

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float buttonLifetime = 1.5f;
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(6f, 3f);

    private GameObject activeButton;
    private int activeIndex = -1;

    public KeyCode ExpectedKey => activeIndex >= 0 ? Keys[activeIndex] : KeyCode.None;
    public bool HasActiveButton => activeButton != null;

    private float timer;

    int score = 0;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multText;
    int scorePerHit = 100;
    int scorePerMiss = -100;
    private float multiplierGain = 0.05f;
    private float maxMultiplier = 10f;

    private float multiplier = 1f;
    private int combo;

    public float Multiplier => multiplier;
    public int Combo => combo;
    public int Score => score;


    [SerializeField] private float runDuration = 30f;

    [SerializeField] private float startInterval = 0.9f;
    [SerializeField] private float endInterval = 0.25f;

    [SerializeField] private float startLifetime = 1.4f;
    [SerializeField] private float endLifetime = 0.45f;

    [SerializeField] private AnimationCurve difficultyCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private float elapsed;

    private float Difficulty => difficultyCurve.Evaluate(Mathf.Clamp01(elapsed / runDuration));
    private float CurrentInterval => Mathf.Lerp(startInterval, endInterval, Difficulty);
    private float CurrentLifetime => Mathf.Lerp(startLifetime, endLifetime, Difficulty);


    private void Update()
    {
        elapsed += Time.deltaTime;

        if (activeButton != null)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= CurrentInterval)
        {
            Spawn();
            timer = 0f;
        }
    }

    private void Spawn()
    {
        if (buttonPrefab == null) return;

        Vector3 position = transform.position + new Vector3(
            Random.Range(-spawnAreaSize.x * 0.5f, spawnAreaSize.x * 0.5f),
            Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f),
            0f);

        activeButton = Instantiate(buttonPrefab, position, Quaternion.identity, transform.parent);
        activeIndex = Random.Range(0, Keys.Length);

        Image image = activeButton.GetComponent<Image>();

        if (image != null && sprites != null && activeIndex < sprites.Length)
        {
            image.sprite = sprites[activeIndex];
        }


        ButtonBehaviour behaviour = activeButton.GetComponent<ButtonBehaviour>();
        behaviour.Initialize(Keys[activeIndex], sprites[activeIndex], CurrentLifetime);
        behaviour.Resolved += OnButtonResolved;
    }

    public void ClearActiveButton()
    {
        if (activeButton != null)
        {
            Destroy(activeButton);
        }

        activeButton = null;
        activeIndex = -1;
    }

    private void OnButtonResolved(ButtonBehaviour button, bool correct)
    {
        button.Resolved -= OnButtonResolved;

        if (correct)
        {
            score += Mathf.RoundToInt(scorePerHit * multiplier);
            combo++;
            multiplier = Mathf.Min(multiplier + multiplierGain, maxMultiplier);
        }
        else
        {
            score += scorePerMiss;
            combo = 0;
            multiplier = 1f;
        }

        scoreText.text = score.ToString();
        multText.text = multiplier.ToString();
        ClearActiveButton();
    }
}
