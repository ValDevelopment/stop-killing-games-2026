using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private KeyCode expectedKey;
    private float lifetime;
    private float age;
    private bool resolved;

    public KeyCode ExpectedKey => expectedKey;
    public float RemainingNormalized => lifetime <= 0f ? 1f : Mathf.Clamp01(1f - age / lifetime);

    public event Action<ButtonBehaviour, bool> Resolved;

    public void Initialize(KeyCode key, Sprite sprite, float lifetimeSeconds)
    {
        expectedKey = key;
        lifetime = lifetimeSeconds;
        age = 0f;
        resolved = false;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null && sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }

    private void Update()
    {
        if (resolved) return;

        if (CheckInput()) return;

        if (lifetime <= 0f) return;

        age += Time.deltaTime;

        if (age >= lifetime)
        {
            Resolve(false);
        }
    }

    private bool CheckInput()
    {
        for (int i = 0; i < ButtonSpawner.Keys.Length; i++)
        {
            KeyCode key = ButtonSpawner.Keys[i];

            if (!Input.GetKeyDown(key)) continue;

            Resolve(key == expectedKey);
            return true;
        }

        return false;
    }

    private void Resolve(bool correct)
    {
        resolved = true;
        Resolved?.Invoke(this, correct);
    }
}
