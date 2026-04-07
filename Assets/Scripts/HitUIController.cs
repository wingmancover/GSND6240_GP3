using UnityEngine;
using UnityEngine.UI;

public class HitUIController : MonoBehaviour
{
    [Header("Heart Images")]
    public Image[] hearts;

    [Header("Sprites")]
    public Sprite heartFillSprite;
    public Sprite heartEmptySprite;

    public void ResetHearts()
    {
        UpdateHearts(0);
    }

    public void UpdateHearts(int hitCount)
    {
        if (hearts == null || hearts.Length == 0)
        {
            return;
        }

        // hitCount:
        // 0 = F F F
        // 1 = F F E
        // 2 = F E E
        // 3 = E E E
        // 4 = game over

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null)
            {
                continue;
            }

            hearts[i].sprite = heartFillSprite;
        }

        if (hitCount >= 1 && hearts.Length >= 3)
        {
            hearts[2].sprite = heartEmptySprite; // right
        }

        if (hitCount >= 2 && hearts.Length >= 2)
        {
            hearts[1].sprite = heartEmptySprite; // middle
        }

        if (hitCount >= 3 && hearts.Length >= 1)
        {
            hearts[0].sprite = heartEmptySprite; // left
        }
    }
}