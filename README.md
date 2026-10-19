# CS464 Assignment 02 · Aman Fatima · BSCS25087

##  I have made the video of three levels because it becomes to long.
- **Video:** [https://drive.google.com/file/d/15xv5oZGAQLpFblQnFHnUdwFhXn0bgS8S/view?usp=drive_link]

- **Input:** Legacy Input using `Input.GetAxis()` and `Input.GetKeyDown()`, with Active Input Handling set to Both.
- **Ground check:** `Physics.CheckSphere()` using the Ground layer.
- **Camera:** `Vector3.Lerp()` with a smoothing speed of 5 and a 2 m look-ahead.
- **Juice:** The player stretches when jumping and squashes when landing. An EaseInOut curve controls the effect.

## Tuning

| Setting | Value | Why this number |
|---|---|---|
| Speed | 5 m/s | Gives the player controllable movement. |
| Jump height | 8 m | Lets the player reach higher platforms. |
| Time to apex | About 1.28 s | Estimated using the jump height and normal gravity. |
| Fall multiplier | 2× | Makes falling faster and the jump feel responsive. |
| Coyote time | 0.15 s | Allows a small window to jump after leaving an edge. |
| Jump buffer | 0.35 s | Remembers the jump input briefly before landing. |
| Camera smoothing | 5 | Smooths the camera movement using `Vector3.Lerp()`. |
| Look-ahead | 2 m | Shows more of the area in front of the player. |

## Four answers

### 1. The jump, in numbers.

The jump height is 8 m. With normal gravity, the launch speed is about 12.52 m/s and the time to reach the top is about 1.28 seconds. The falling time is also about 1.28 seconds without extra fall gravity. The actual gap the player can cross depends on the movement speed and level layout.

### 2. Update and FixedUpdate.

`Update()` reads the movement and jump input. `FixedUpdate()` handles Rigidbody movement and jumping. If physics is handled in `Update()`, it can become inconsistent because `Update()` does not run at a fixed time interval.

### 3. Vector3.Dot.

`Vector3.Dot()` is not used in my current controller. It could compare the movement direction with the player's forward direction. A positive result means forward, zero means sideways, and a negative result means backward.

### 4. The juice.

The player stretches when jumping and squashes when landing. The effect uses an EaseInOut curve and lasts 0.15 seconds, which is about 9 frames at 60 FPS. It makes jumping and landing feel more responsive.

## Levels

| Level | What the controller must do there | What I changed for this jump |
|---|---|---|
| Level01 | Moving from spawns to goal using camera relative controls. | Used the shared PlayerController settings. |
| Level02 | moving from spawns to goals room that are at differnt rooms. | Used the same jump settings. |
| Level03 | moving and jumping from spawns to goals room that are at differnt rooms. | Used the same jump settings. |
| Level04 | Move and jump around buildings to reach at the goal. | Used the same jump settings. |
| Level05 | moving through jungle explored differnt things and raech to the goal. | Used the same jump settings. |

The same Player prefab and PlayerController are used in all five levels. No separate jump settings are used for individual levels.

---

**CS464 · ITU · Fall 2026**
