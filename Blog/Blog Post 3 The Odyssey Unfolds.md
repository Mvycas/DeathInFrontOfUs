---
Title: "Blog Post 3: The Odyssey Unfolds - Inside look at the First Chapters of 'DeathInFrontOfUs' Development"
Post creation date: "17-03-2024"
Author: Martynas Vycas
---

## Current Gameplay:
  
[![Gameplay](http://img.youtube.com/vi/FCB5tSaAnJY/0.jpg)](http://www.youtube.com/watch?v=FCB5tSaAnJY "Video Title")

In the gameplay video provided above, you can observe the implementation of the following systems:

- Health system
- Combat (Shooting) system
- Movement system

In addition to these systems, I've also introduced several key animations that significantly enhance the visual experience:

- Muzzle flash
- Gore effect (Flesh-spreading upon hitting the zombie)
- Player movement
- Player jump 

In this blog post, we're going to delve into the intricacies of how some of these systems and features were built.

## Health system

The foundation of the health system is the Health class. It encapsulates health management logic, including damage reception and healing capabilities. Here's a closer look:

```c#
public class Health
{
    public event Action OnDeath;
    private float CurrentHealth { get; set; }
    private float MaxHealth { get; set; }

    public Health(float maxHealth)
    {
        CurrentHealth = MaxHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }
}
```

'TakeDamage' method demonstrates an event-driven approach to character's death, which allows for a loosely coupled system, where the health class does not need to know about specifics of what needs to happen when certain character dies. Instead it broadcasts the 'OnDeath' event.

The use of the ? operator before .Invoke() is a safety measure to prevent attempting to call Invoke() on a null reference. In C#, delegates can be thought of as a list of methods to call. However, if no methods have been added to this list (meaning the delegate is null), attempting to invoke it would result in a NullReferenceException. 

Currently, this system is implemented for zombies, where the OnDeath event simply removes the game object from the scene:

```c#
public class Enemy : Character
{
    protected override void Die()
    {
        Debug.Log("Zombie died");
        Destroy(gameObject);
    }
}
```
### Character Class

Frome the code snippet above we can notice that class Enemy is derived from class Character.

Character class serves as an abstract base, ensuring all characters can interact with the health system through a unified interface. Here's how it's implemented:

```c#
public abstract class Character : MonoBehaviour, IDamageable
{
    [SerializeField] private CharacterData characterData;
    private Health _health;

    protected virtual void Awake()
    {
        _health = new Health(characterData.maxHealth);
        _health.OnDeath += Die;
    }

    public void ApplyDamage(float damage)
    {
        _health.TakeDamage(damage);
    }

    protected abstract void Die();
}

```

The class uses [SerializeField] attribute to link with CharacterData, a ScriptableObject that stores character specific data such as maximum health. (Can be extended) This allows for an easy adjustments to character properties directly from the Unity Editor without altering the code.

In the Awake method, the class initialize Health object with the maximum health value defined in CharacterData scriptable object.
Morevoer, It also subscribes to the Health object's OnDeath event with the abstract Die method. This setup implies that when the Health object triggers the OnDeath event (when health reaches zero or below), the specific character's Die method will be called.

Scriptable object:

```c#

namespace ScriptableCharactersData
{
    [CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character Data", order = 51)]
    public class CharacterData : ScriptableObject
    {
        public float maxHealth = 100;
        // add more character properties later on such as dmg... exp.. stamina??
    }
}

```

The [CreateAssetMenu] attribute allows to create new instances of the CharacterData ScriptableObject directly from the Unity Editor's "Create" menu, making creation of new character data objects a little faster. 

In summary using an abstract class and an interface allows for an easy addition and customization of character types, while events for death handling reduce system interdependencies. ScriptableObjects simplify the process of adjusting character data dynamically.


## Combat system

The combat system currently features only a single weapon, an UZI, with no melee combat implemented. Bullets are launched as 3D projectiles. Initially it was managed through straightforward object instantiation and removal, but to improve the performance, a transition was made to an object pooling strategy. This significantly reduces the computational load on CPU by reusing game objects (bullets) instead of constantly creating and destroying them. This optimization was not in response to existing lag but was adopted as a proactive measure to ensure smooth gameplay by any means.

### Bullet pooling:

```c#

public class BulletPool : MonoBehaviour
    {
        public static BulletPool SharedInstance { get; private set; }
        public List<GameObject> pooledBullets;
        public GameObject bulletPrefab;
        public int bulletAmount = 20;

        private void Awake()
        {
            SharedInstance = this;
            pooledBullets = new List<GameObject>();
            for (var i = 0; i < bulletAmount; i++)
            {
                var tmp = Instantiate(bulletPrefab);
                tmp.SetActive(false);
                pooledBullets.Add(tmp);
            }
        }

        public GameObject GetPooledBullet()
        {
            return pooledBullets.FirstOrDefault(t => !t.activeInHierarchy);
        }
    }

```

- SharedInstance: A public static property that holds a singleton instance of the BulletPool class, ensuring that only one instance of the bullet pool exists in the game at any time. This allows other scripts to easily access the bullet pool without needing to find or create a new instance.

- pooledBullets: A public list of GameObjects that stores the pool of bullet objects. 

- bulletPrefab: A public GameObject that specifies the prefab to be used for creating bullets in the pool.

- Awake: It sets the SharedInstance to "this" BulletPool instance, creates a specified (MAX) number of bullets from the bulletPrefab, deactivates them (to make them invisible and inactive in the game), and stores them in pooledBullets list.

- GetPooledBullet: searches through the "pooledBullets" list and returns the first bullet that is not currently active in the game. The FirstOrDefault function is a LINQ method that returns the first element in a sequence that satisfies a specific condition. In this case, the condition is !t.activeInHierarchy, which means it checks if the bullet is inactive in the game hierarchy. If no such bullet is found, it returns null. If inactive bullet is found, then it returns that gameObject. (bullet)

The Attack() method in the Combat class handles player-initiated attacks. It activates the muzzle flash effect to simulate gunfire and then triggers the gun to shoot a bullet projectile. The _timestamp variable is updated to enforce a cooldown period between attacks.

```c#
private void Attack()
        {
            gun.muzzleFlashObject.SetActive(true);
            gun.ShootBullet();
            _timestamp = Time.time + timeBetweenShots;
        }
```

### A glimpse inside the Gun class:

```c#

public class Gun : MonoBehaviour
    {
        public Transform barrelEnd; 
        public float bulletSpeed = 100f;
        public float spreadAngle = 5f; // Degrees of spread
        public GameObject muzzleFlashObject; // Assign muzzle flash in the editor
    
        private void Awake()
        {
            // Ensure the muzzle flash is disabled on start
            muzzleFlashObject.SetActive(false);
        }
        
        public void ShootBullet()
        {
            GameObject bullet = BulletPool.SharedInstance.GetPooledBullet();
            if (bullet == null) return;
            // Calculate random spread for the shoot direction
            Vector3 shootDirection = Quaternion.Euler(
                0, // Pitch
                Random.Range(-spreadAngle, spreadAngle), // Yaw
                0 // Roll is not needed for bullets
            ) * barrelEnd.forward;

            bullet.transform.position = barrelEnd.position; // Set the bullet's position
        
            bullet.SetActive(true); // Activate the bullet

            // Apply force to the bullet Rigidbody
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero; // Reset the bullet's velocity before applying a new force
            rb.AddForce(shootDirection * bulletSpeed, ForceMode.VelocityChange);
        }
    }

```

- ShootBullet() method:
 
 BulletPool.SharedInstance.GetPooledBullet() - retrieves an inactive bullet GameObject from the BulletPool using the GetPooledBullet method. If bullet is null, it means there are no active bullets available in the pool and method exits early.

shootDirection - calculates the random direction in which the bullet will be fired, simulating the spread caused by recoil action. By applying a random angle within the specified range to the yaw axis, the bullets are distributed in a wider area.

bullet.transform.position - sets the bullet's initial position to the gun's barrelEnd.

bullet.SetActive(true) - activates the bullet.

`bullet.GetComponent<Rigidbody>()` - Retrieves the Rigidbody component attached to the bullet GameObject. This component is necessary for applying physics forces.

rb.velocity = Vector3.zero - Resets the bullet's velocity to ensure that the bullet starts with a consistent initial velocity, unaffected by any previous motion. (since we are reusing objects)

rb.AddForce(shootDirection * bulletSpeed, ForceMode.VelocityChange); - Applies a force to the bullet's Rigidbody component to propel it in the calculated shootDirection with a magnitude determined by the bulletSpeed

ForceMode.VelocityChange - Ensure that bullet speed is set to the desired value instantly, moving bullet at the intended speed from the start.

### Bullet class:

```c#

        public float damageAmount = 10f; 
        public GameObject goreEffectPrefab; 
        public float lifetime = 1f; 

        private float _timeSinceActivated; 

        private void OnEnable()
        {
            _timeSinceActivated = 0f; 
        }
      
        private void Update()
        {
            // Update the timer
            _timeSinceActivated += Time.deltaTime;

            // Check if the bullet has existed longer than its lifetime
            if (_timeSinceActivated >= lifetime)
            {
                DisableBullet();
            }
        }
        
   
        private void OnCollisionEnter(Collision collision)
        {
            // Only get the Character component if the collision object has the correct tag
            if (collision.gameObject.CompareTag("Zombie"))
            {
                var character = collision.collider.GetComponent<Character>();
                if (character != null)
                {
                    character.ApplyDamage(damageAmount);
                }

                // Instantiate the gore effect at the collision point and orient it correctly
                var goreEffectInstance = Instantiate(goreEffectPrefab, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
                var ps = goreEffectInstance.GetComponent<ParticleSystem>();
                ps.Play();

                // Destroy the gore effect after it has finished
                Destroy(goreEffectInstance, ps.main.duration);
            }

            // Deactivate the bullet in any case
            DisableBullet();
        }

        private void DisableBullet()
        {
            // Deactivate the bullet
            gameObject.SetActive(false);
        }

```

Update(): the bullet is checked every frame to determine if it has been active longer than its specified lifetime, and if it has, it is disabled. As a result, bullet retention will take place faster and will not require the bullet to wait until it finally collides with an object that has a collider attached. In a way, it simulates the maximum bullet travel distance, but based on time alone. Previously, the bullet distance was calculated after it was shot and checked to ensure that it did not exceed a maximum set distance. Although it was an expensive operation, the current method is much simpler and significantly less expensive.

OnCollisionEnter(Collision collision): verifies whether the collision involves an object tagged as "Zombie". If so, it inflicts damage to the character component of the collided object. Additionally, it creates a gore effect at the point of collision, initiates its particle system, and removes it after its duration elapses. Regardless of the collision outcome, it deactivates the bullet. (So that it would still disable the bullet even if it hits a collider without the tag "zombie")


# Sources:

- https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.firstordefault?view=net-8.0

- https://docs.unity3d.com/ScriptReference/GameObject-activeInHierarchy.html

- https://www.kodeco.com/847-object-pooling-in-unity

# Resources used: 

- Megan (Player character): https://www.mixamo.com/#/?page=1&query=megan&type=Character
- Animations: https://www.mixamo.com/
- Map & Environment: https://assetstore.unity.com/packages/3d/environments/flooded-grounds-48529
- Gore effect: https://assetstore.unity.com/packages/vfx/particles/blood-gush-73426
- Trees: https://assetstore.unity.com/packages/3d/vegetation/trees/conifers-botd-142076
- Submachine Gun (UZI): https://assetstore.unity.com/packages/3d/props/guns/submachine-gun-20746
- Zombies: https://assetstore.unity.com/packages/3d/characters/humanoids/zombie-30232

