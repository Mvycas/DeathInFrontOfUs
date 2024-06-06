---
Title: "Blog Post 1 Hello world! (Roll a ball game)"
Post date: "17-02-2024"
Author: Martynas Vycas
---

# Introduction

The first couple of game development classes I took introduced me to Unity, scripting in C#, and MDA framework.

Our first assignment or short homework project, I am not sure what to call it, involved us creating a rollaball game from Unity tutorials with the option of extending it - adding a few additional features - at the teachers’ request. I will therefore dedicate this blog post to the additional tweaks that I have made and a playable demonstration of the game.

# Extra features

- Hot plate - a platform that automatically disables when the player touches it, and it also changes color hue to indicate when it is about to be disabled.
- Moving platforms - Nothing fancy, just a couple of platforms that move based on the Y axis, with a time delay between.
- Map and obstacle design - I designed a short obstacle route that the player must follow to collect the yellow gems and complete the game.
- Lava ground - If the player falls off the map it respawns at the position where the last collectible was located (the yellow gem).
- 
Additionally, I have found and added some prefabs from SketchFab.com for the visuals.

## Hot plate

One of the most interesting features was the “Hot plate”. Below is an explanation of the platformDisable.cs script, 
which is responsible for the mechanics/logic of the “Hot plate” feature.

```c#
 private void Start()
{
    objectRenderer = GetComponent<Renderer>();
    objectCollider = GetComponent<MeshCollider>();
    initialColor = objectRenderer.material.color;
}
```

The start method simply retrieves the component renderer, colider, and color of the game object to which this script is attached.


```c#
private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        // Start the coroutine
        StartCoroutine(Blink());
    }
}
```

Essentially, OnTriggerEnter detects whether the collided object is a “Player”, or, more precisely, whether the collided object has a player tag, and if it does, it begins the coroutine with method Blink(). I have one Player game object, so tag may not be necessary here, but I already had them assigned after the tutorial, and OnTriggerEnter is not a performance-critical function, so I believe it is fine.

Moving forward, the Blink method:

```c#
IEnumerator Blink()
{
    // Fade out // red color shift
    float elapsedTime = 0f;
    while (elapsedTime <img fadeDuration)
    {
        elapsedTime += Time.deltaTime;
        float lerpFactor = elapsedTime / fadeDuration;
        objectRenderer.material.color = Color.Lerp(initialColor, targetColor, lerpFactor);
        yield return null;
    }
    // Fade out // red color shift


    objectCollider.enabled = false;
    objectRenderer.enabled = false;

    // Delay 
    yield return new WaitForSeconds(blinkInterval / 2f);

    // Enable platform after delay
    objectCollider.enabled = true;
    objectRenderer.enabled = true;

    objectRenderer.material.color = initialColor;
    }
}
```

The first section fades out the object by gradually changing its color from its initial color to its target color. (In my case from white to red)

In this case, the effect is applied over a specified fadeDuration time (that can be set in the inspector).

In computer graphics, Lerp (Linear interpolation) is used widely to find values between two endpoints on a linear progression (in my case between white and red, while progressing in time).

Then after the while loop is completed it disables the collider and renderer of the gameObject. Usually we write something like “gameObject.SetActive(false)”, but in this case it would destroy the whole object with the script, and it would not be possible to re-enable it during the runtime. Therefore, the correct solution here is to disable and enable collider with renderer.

# Game Demo

- Move - WASD | arrow keys
- Jump - spacebar

Link to playable webGL build: https://mvycas.github.io/Homework-RollABall/

# Conclusion

“Every journey begins with a single step.” – Lao Tzu

As my first assignment, I treated it merely as a playground, trying to implement various things without focusing on quality. It was my primary goal here to become familiar with Unity and to develop something fun. That is exactly what I did! This little step marks the beginning of my journey…

# Game map

![Map](https://github.com/Mvycas/DeathInFrontOfUs/blob/main/Blog/gameMap.png)

# Original blog post

This is the link to the blog that I initially started, but later it was moved to MD document, because it was part of requirements for this project.

https://blog-gamedev.vercel.app/post/helloworld/
