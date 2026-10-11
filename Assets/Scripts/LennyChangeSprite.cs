using UnityEngine;
using System.Collections;

public class LennyChangeSprite : MonoBehaviour
{
    public SpriteRenderer sprite;

    public Sprite idle,
        walk1,
        walk2,
        jump,
        doubleJump,
        climb1,
        climb2,
        glide,
        atkFront,
        atkBack,
        atkUp,
        atkDown,
        breathFire;
    
    public float cycleSpeed = 0.001f;
    public bool cycle = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void ChangeSprite(Sprite newSprite)
    {
        sprite.sprite = newSprite;
    }

    public void ReturnSprite()
    {
        sprite.sprite = idle;
    }


    public void CycleSprites(Sprite s1, Sprite s2)
    {
        SpriteRenderer sprite1 = GetComponent<SpriteRenderer>();
        sprite.sprite = s1;
        sprite1.sprite = s2;
        StartCoroutine((CycleSpritesT(s1, s2, sprite1)));
    }
    
    public IEnumerator CycleSpritesT(Sprite s1, Sprite s2, SpriteRenderer sr)
    {
        var waitForSeconds = new WaitForSeconds(cycleSpeed); // Optimized caching

        while (cycle)
        {
            yield return waitForSeconds;
            sprite.enabled = !sprite.enabled;
            sr.enabled = !sr.enabled;
        }
    }
}


