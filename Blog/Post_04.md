---
Title: "Blog Post 4: Development continues"
Post creation date: "17-04-2024"
Author: Martynas Vycas
---

This blog post will shortly review the following features and systems that has been developed and added since the last time: 

- Object pooling script made more generic (to reuse)
- Zombie pool added
- Zombie spawning system
- Menu camera zoom
- Object obstuction/occlusion systems
- Transparent surface Lit shader (Shader graph)

## Object pooling

To begin with the latest updates was the object pooling system. I already had object pool for bullets, although I figured at this stage that I might need pooling for zombies as well, since destroying and re-creating them after they get shot/killed would be processor intensive task and performance-wise it is better to have object pool for such occasions.
Therefore the previous object pooling script was adjusted, and made more generic:

```c#
namespace ObjectPoolingSystem

{
    public class ObjectPool : MonoBehaviour
    {
        public GameObject prefabToPool;
        public int poolSize = 20;
        private List<GameObject> _pooledObjects;

        private void Awake()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            _pooledObjects = new List<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefabToPool);
                obj.SetActive(false);
                _pooledObjects.Add(obj);
            }
        }

        public GameObject GetPooledObject()
        {
            return _pooledObjects.FirstOrDefault(t => !t.activeInHierarchy);
        }
        
        public int CountActiveObjects()
        {
            return _pooledObjects.Count(obj => obj.activeInHierarchy);
        }
    }
}
```

The changes include having prefabToPool which is the game object which we want to initialize within the specific pool that we create. The object and pool size can be set from the editor.
Other than that, during the Awake stage - when the script instance is being loaded (before the game starts) - we initialize and populate gameObject list. After the pool is initialized, the pooled object can be retrieved by GetPooledObject method. The CountActiveObjects is neccessary to have so that we could know in object spawn manager if any more objects can be initialized at specific stage.

### Zombie spawn manager 

For instance, this class is used in conjunction to the zombie spawn manager, below is an overview of SpawnZombie method: 

ZombieSpawn.cs:

```c#
private void SpawnZombie()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        GameObject zombie = zombiePool.GetPooledObject();
        if (zombie != null)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            zombie.transform.position = spawnPoint.position;
            zombie.transform.rotation = spawnPoint.rotation;
            zombie.SetActive(true);
        }
    }
```

All this does is checks if there are any spawn points, and then gets a zombie object from the zombie pool and sets it active at the spawn point location.
This method is getting called in the same class under Update: 

ZombieSpawn.cs:

```c#
 private void Update()
    {
        int activeZombiesCount = zombiePool.CountActiveObjects();
        if (activeZombiesCount < maxZombies)
        {
            SpawnZombie();
        }
    }
```
## Camera occlusion obstruction systems

Other important feature that was developed at this stage was the camera occlusion. This was essential because the game features a fixed, angled, top-down
main camera and a complex map with high objects. This means that at some point there will be an issue of certain objects obstructing the 
view - getting in between the camera and main character object. This issue were fixed by developing few systems that are used in different occasions:
    - Transparency with zoom
    - Transparency 
    - Object diffusion 
    
The following gameplay showcase how each system works:

[![OcclusionSystems](https://img.youtube.com/vi/NUkRqnatG0Y/0.jpg)](https://www.youtube.com/watch?v=NUkRqnatG0Y "Video Title")

The object diffusion system was hardest to implement and it is far the most interesting one.

### Shader graph

To begin with, a lit surface shader was created within shader graph editor, which can be seen in the picture below:

[INSERT PICTURE LATER ON]

At first I tried to re-create a lit surface shader, that would take the exactly same values and textures as URP/Lit shader would:

- Base map/Albedo (With intensity adjustment)
- Metallic map (with smoothness adjustment)
- Normal Map
- Occlusion map
- Emission 

I named this shader: "HeightBasedTransparnecy".

The "Sample Texture2D" nodes are used to read colors and other data such as hue saturation and etc. It returns a texel (color) value of the tecture for the given coordinate. 
The multiply nodes combine inputs to mix the textures. For example to adjust intensity of Albedo or to adjust smoothness for metallic map.

Position Node set to world makes sure that the effect depend on the actual position within the scene and not to the object or a camera. It enables shader to
apply transparency effect based on the object's (to which this shader is attached) location in the world space.

The split RGB (XYZ) takes output from the position node. It takes G (Green) channel, which eventually is the Y-axis, that represents vertical height in the world space.

Step crates a binary output based on comparison between the inputs. It checks whether the height (Y value of Position node) is below or above the height threshold.

It returns one and zero. 0 - fully opaque below the threshold. 1 - fully transparent above the threshold.
This then goes into alpha channel which controls the visibility of the shader.

### Visibility controller

After creating this shader, specific buildings that had to be obstructed were modified by changing URP/Lit shader to the one that "HeightBasedTransparnecy" shader 

With this shader, the heightThreshold value can be controlled by a custom script (manager) when certain conditions are met.
In this case it is being controlled by the BuildingVisibilityController.cs. It is attached to a specific object along with a collider 
(isTrigger must be set) which controls the visibility of the building materials by adjusting the height threshold for each of them. 
This is controlled based on whether the player is nearby or inside the building. (Colliders)

BuildingVisibilityController.cs:

```c#

private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(InterpolateHeightThreshold(targetThreshold, false, enterDuration));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(InterpolateHeightThreshold(targetThreshold, true, exitDuration));
        }
    }

    private IEnumerator InterpolateHeightThreshold(float targetValue, bool reverting, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            foreach (Material material in buildingMaterials)
            {
                if (material.HasProperty("_HeightThreshold"))
                {

                    float startValue = reverting ? targetValue : _initialThreshold;
                    float endValue = reverting ? _initialThreshold : targetValue;

                    float newThreshold = Mathf.Lerp(startValue, endValue, time / duration);
                    material.SetFloat("_HeightThreshold", newThreshold);
                }
            }
            time += Time.deltaTime;
            yield return null;
        }
    }

    private void ResetMaterialThresholds(float value)
    {
        foreach (Material material in buildingMaterials)
        {
            if (material.HasProperty("_HeightThreshold"))
            {
                material.SetFloat("_HeightThreshold", value);
            }
        }
    }
```
OnTriggerEnter is set to detect collisions with a Player collider. 
If it does detect such collision, it start a coroutine of InterpolateHeightThreshold with reverting set to false. 
The reverting does tell if the shader transparency should go back to the previous values, as to "revert" to full building visibility. 
It also Stops all couroutines, to avoid unexpected behavior when rapidly entering or exiting the trigger zones. 
This ensures a smooth transition without conflicts or glitches, considering that each coroutine takes time to interpolate the value. 

Inside the InterpolateHeightTreshold it does set start and end values based on the condition - if it is reverting or not. 
If it is not reverting, the start value "_initialThreshold" would be set to 53 and the endValue the "targetValue" would be set to 20. 

On exit those values would just flip and eventually it would lerp (slowly interpolate) to the target values. 

The ResetMaterialThresholds called within OnDestroy makes sure that all the preset values of the shaders are being reset to the initial ones before destroying the object, 
for example during the scene reloads (Start new game). This is essential to reset otherwise if the player is in the building that has this obstruction script and he initializes "new game" - 
the initial treshold state before entering the building would be lost. In such case the building would be set to 20 - the threshold which is the target when the player is within the building. 

## Pause view

For the pause View I wanted a dynamic solution,
where the camera would transform in a way so the player would see his character on one side and menu on the other. 
Sort of a copy of the menu from one of my favorite games - Dying light.
just so that in my case In my case, if a player presses the escape button at any point during the game, 
it transitions exactly within the location where the player is. There is only a change in the camera. 
This was implemented by attaching a second camera to the main character that is always following, and it would be set so that it depicts the side of the character. 

Within CameraController.cs the transition script as follows:

```c#
private IEnumerator TransitionToPauseView()
        {
            IsTransitioning = true;
            while (Vector3.Distance(transform.position, pauseViewTransform.position) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, pauseViewTransform.position, Time.unscaledDeltaTime * 3);
                transform.rotation = Quaternion.Lerp(transform.rotation, pauseViewTransform.rotation, Time.unscaledDeltaTime * 3);
                yield return null;
            }

            IsTransitioning = false;
            _currentTransition = null; 
        }
```

- transform.position - original angled camera position.
- pauseViewTransform.position - pause view camera.

It Interpolates position and rotation between these two cameras.
The important thing why I wanted to mention this feature within the blog post is that the time used here for interpolation is an unscaled time. 
It means that when we actually pause the game by setting Time.timeScale to 0, the interpolation between the cameras during the pause state would not interrupt and brake. 
It would finish interpolating while pretty much every other process within the game would be at a stop/pause. 


# Sources:

- https://forum.unity.com/threads/how-to-replicate-the-lwrp-standard-shader-in-shadergraph.539089/

- https://docs.unity3d.com/Packages/com.unity.shadergraph@6.9/manual/Sample-Texture-2D-Node.html






