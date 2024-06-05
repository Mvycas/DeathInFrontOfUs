---
Title: "Blog Post 5: At the end of the journey"
Post date: "07-05-2024"
Author: Martynas Vycas
---

This blog post will shortly review some of the following features and systems that has been developed and added since the last time: 

- Zombie AI system
- Crates
- Some Animations 
- Game State
- Arrow indicator
- UI panels

## Zombie AI

This time the focus were on finishing the game loop, so to make the game have some sort of state so if the player dies, for example, 
the system would eventually stop the game (Time.timeScale = 0) and show certain UI. The zombie Ai system has been developed with a help of Unity Course.
(Link in sources) That was a great help in developing AI that I could say is a decent one. (still not a very smart one :)) 

The AI behavior for zombies is managed by finite state machine (FSM). 

The AIState class is a super class and serves as a blueprint for individual states. It defines the structure and lifecycle of each state:

```c#
 public class AIState
    { 
        ...

        public AIState(GameObject _zombie, NavMeshAgent _agent, Animator _anim, Transform _player)
        {
            zombie = _zombie;
            agent = _agent;
            anim = _anim;
            stage = EVENT.ENTER;
            player = _player;
        }

        public virtual void Enter()
        {
            stage = EVENT.UPDATE;
        }

        public virtual void Update()
        {
            stage = EVENT.UPDATE;
        }

        public virtual void Exit()
        {
            stage = EVENT.EXIT;
        }

        public AIState Process()
        {
            if (stage == EVENT.ENTER) Enter();
            if (stage == EVENT.UPDATE) Update();
            if (stage == EVENT.EXIT)
            {
                Exit();
                return nextState;
            }
            return this;
        }

        ...
    }
```

Using virtual modifier gives a possibility for the before-mentioned derived states to override the base class tailoring state specific behavior.

In the AIController, that is attached to the zombie object, the Idle state is being initiated as such:

AIController.cs:
```c#
private void Start()
  {
    currentState = new Idle(this.gameObject, agent, anim, player);
  }
```

It creates (initializes) the Idle state, passing references to the zombie GameObject, NavMeshAgent, Animator, and the player's Transform.

Idle.cs: 
```c#
public override void Enter()
  {
      anim.SetTrigger("idle");
      base.Enter();
  }
```

It sets animation to idle in the zombie's Animator. Eventually base.Enter() is called which is AIState (super class) method that completes any standard initialization. 
In this case it sets stage to EVENT.UPDATE. After which the state machine runs the Update method within the same derived Idle class. 

AIState.cs:
```c#
 public virtual void Enter()
  {
    stage = EVENT.UPDATE;
  }
    public virtual void Update()
  {
    stage = EVENT.UPDATE;
  }
```
The update inside Idle class will be continously called within Idle state, until the Idle is active:

Idle.cs:
```c#
 public override void Update()
        {
            if (CanSeePlayer())
            {
                nextState = new Chase(zombie, agent, anim, player);
                stage = EVENT.EXIT;
            }
            nextState = new Patrol(zombie, agent, anim, player);
            stage = EVENT.EXIT;
            
        }
```

It does check if it can see the player (method called from the AIState.cs)
and if it can - the next state is being set/instantiated to Chase. 
After which EVENT.EXIT triggers cleanup operations within idle state and exit operation within super class:

Idle.cs:
```c#
 ...
public override void Exit()
        {
            anim.ResetTrigger("idle");
            base.Exit();
        }
```

- anim.ResetTrigger("idle") - to prevent any animation glitches. If it does not get reset after leaving the idle state, the zombie might be stuck within idle animation regardless.

base.Exit() calls Exit() method of a superclass:

AIState.cs:

```c#
...
public virtual void Exit()
        {
            stage = EVENT.EXIT;
        }

        public AIState Process()
        {
            if (stage == EVENT.ENTER) Enter();
            if (stage == EVENT.UPDATE) Update();
            if (stage == EVENT.EXIT)
            {
                Exit();
                return nextState;
            }
            return this;
        }

```

This sets stage to EVENT.EXIT which will eventually trigger to return nextState that was set to Chase.
The Process is being triggered from AIController and it is being called within Update.

AIState provides a reusable structure for even more states if to be developed, as it allows to add even more states without affecting the core state management system. 

### Animation

Below you can see the Animator of Zombie character:

![ZombieAnimator](https://github.com/Mvycas/DeathInFrontOfUs/blob/main/Blog/ANIMATE.png)

In total there are 5 triggers.

When the state is being changed from idle to Chase,
in the Enter method of the Chase.cs there would be such line "anim.SetTrigger("idle");" 
This would trigger the running animation.

Has exit time is unchecked for all the animations, so it would not appear laggy, and transition right away. 
For example, if it is not unchecked, the "lag" can be noticed when "die" animation "fallingBack" is being played - 
When at some point zombie dies, it still would appear walking or running or idling before it actually dies if the
"HasExitTime" option for the previous animations would be checked.

You can see from the transition graph that most of the animations has sort of double-way binding and FallingBack (dying) does not.
This is because, for example, from attack you can transition into any other state, and from dying you sort of can't, unless the zombie would have revive mechanics. 
Even then, it would probably transition to animation that shows getting up, and from there to any other state. So dying, still would have a one way transition from the other states.

All animations were taken from Mixamo and extracted within the editor later on. 

### Crate

The main objective of the game is to find the medical crate that has an antibiotic vial. The crate FBX model was downloaded from Unity Assets store.
To implement some sort of interactive search and to show the player that the crate is being searched, a progress bar was implemented.
It is attached to the crate and shows "red dot" 2D sprite being circularly loaded. The script for the crate logic is as follows:

crate.cs:

```c#
void Awake()
    {
        _uiCanvas = GameObject.FindGameObjectWithTag("InteractableUI");
    }
    void Start()
    {
        _uiCanvas.SetActive(false);
        _searchDuration = Random.Range(3F, 8F);
        progressBar.fillAmount = 0;
        _isSearching = false;
        wasSearched = false;
        _isWithinCollider = false;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !wasSearched)
        {
            _uiCanvas.SetActive(true);
            _isWithinCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
          _uiCanvas.SetActive(false);
            _isWithinCollider = false;
            if (_isSearching)
            {
                StopCoroutine(SearchCrate());
                progressBar.fillAmount = 0;
                _isSearching = false;
            }
        }
    }

    void Update()
    {
        if (playerInput.SearchInput && !_isSearching && !wasSearched && _isWithinCollider)
        {
            StartCoroutine(SearchCrate());
        }
    }
```

If the player gets close to the crate it does show "interactable object" canva, so to let the player know that this object can be interacted with. 
In this case it shows which button to press on the arcade machine in order to start searching. 

![InteractableUI](https://github.com/Mvycas/DeathInFrontOfUs/blob/main/Blog/interactable_ui.png)


If the button is being pressed and the crate has not yet been searched for, it starts searchCrate() coroutine within Crate.cs class:

Crate.cs:

```c#
private IEnumerator SearchCrate()
    {
        _isSearching = true;
        float elapsedTime = 0;
        while (elapsedTime < _searchDuration)
        {
            if (!playerInput.SearchInput || !_isWithinCollider) 
            {
                progressBar.fillAmount = 0;
                _isSearching = false;
                yield break;
            }
            elapsedTime += Time.deltaTime;
            progressBar.fillAmount = elapsedTime / _searchDuration;
            yield return null;
        }
        EndSearch();
        progressBar.fillAmount = 0;
        wasSearched = true;
        _isSearching = false;
    }
```

While the search duration is passing, it contanstly checks if the player searchInput is true (the button is pressed) and the player remains near the box. During this coroutine it starts filling the before mentioned proggress bar, aka red round sprite giving player a visual information about the search progress. 
If the player during search moves away from the crate, or depresses the button the operation breaks and he can come back and try to search the crate again. 
This mechanic was essential to work correctly, because there might be occasions where the player searching the crate and zombie comes, so he might move in order not to die.

A random search duration is allocated to each crate, ranging between 3 and 8 seconds, and there are only two vials randomly placed within the boxes in the city. 

When the search is finished it calls EndSearch() method:

Crate.cs:
```c#
 private void EndSearch()
    {
        if (containAntiViral)
        {
            GameStateManager.Instance.Victory();
        }
        Debug.Log(containAntiViral ? "Found antivirals!" : "No antivirals here.");
    }
```

Which eventually triggers the victory game state if the crate contained antiviral:

This would eventually lead to operations within the GameStateManager.cs: 

```c#
GameStateManager.cs:
...
 case GameState.Victory:
                Time.timeScale = 0; 
                UIManager.Instance.UpdateUI(state);
                break;
...
```


It would set timeScale to 0, stopping the game and would open up specific "Victory" UI that would be displayed to the user:

![Victory ui](https://github.com/Mvycas/DeathInFrontOfUs/blob/main/Blog/victory.png)

### Arrow navigational indicator

My idea was that the player would go and explore the world while in a constant battle with zombies.
At the end I thought that it may be too problematic since this is still an arcade game and the map is considerably big. 
Therefore I added a 2D navigational sprite arrow that shows the nearest crate from the player:

![Navi_Arrow](https://github.com/Mvycas/DeathInFrontOfUs/blob/main/Blog/arrow2.png)

ArrowIndicator.cs:

```c#
void Update()
    {
        targetCrate = CrateManager.Instance.GetNearestUnopenedCrate(player.position).transform;

        if (targetCrate != null)
        {
            Vector3 targetDirection = targetCrate.position - objTransform.position;
            targetDirection.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, objTransform.up); 
            
            float targetYRotation = targetRotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(90f, targetYRotation, 0f); 
        }
    }
```

It continously calls getNearestUnoponedCrate method within crate manager that calculates the shortest distance to any unopened crate and player at the time. 

Then it calculates the direction to the nearest crate position (targetCrate) while ignoring vertical differences (y). After that it sets object's (arrow's) rotation to look towards the crate.
It rotates ArrowIndicatorCanvas around the Y axis. 
That arrow indicator canvas does have a child element which is the arrow 2D sprite. 
The x axis (pitch) is locked to a fixed 90 degrees since there is no height based calculations that would show if the crate is below the player or above. 
Also for that there would be need of a 3D arrow.

Things regarding the map was not implemented by me, such as physics, sound and etc. Map was downloaded from asset store.
Within map I only changed some building structure and some object placements. Also I re-populated trees, because original ones were just beyond saving. (After conversion to URP their textures were completely broken)
Fun fact, the tree inside the church is accidentaly misplaced, but I left it there because I think it adds up to the abandoned and dead city vibe.

# Sources:

- https://learn.unity.com/course/artificial-intelligence-for-beginners
  
# Content:

![Meme](https://github.com/Mvycas/DeathInFrontOfUs/blob/main/Blog/meme.jpg)




