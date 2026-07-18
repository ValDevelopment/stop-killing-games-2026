using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(6f, 3f);

    private GameObject activeButton;
    private int activeIndex = -1;

    public KeyCode ExpectedKey => activeIndex >= 0 ? Keys[activeIndex] : KeyCode.None;
    public bool HasActiveButton => activeButton != null;

    private float timer;

    private void Update()
    {
        if (activeButton != null)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
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

        SpriteRenderer renderer = activeButton.GetComponent<SpriteRenderer>();

        if (renderer != null && sprites != null && activeIndex < sprites.Length)
        {
            renderer.sprite = sprites[activeIndex];
        }
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
}
