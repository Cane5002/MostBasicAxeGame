using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracker : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    public Color ReadyColor = Color.white;
    public Color CooldownColor = Color.red;

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }

    public void SetCooldown(float cooldown) {
        transform.localScale = new Vector3(cooldown, cooldown, 1);
        if (cooldown == 1) _spriteRenderer.color = ReadyColor;
        else _spriteRenderer.color = CooldownColor;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }
}
