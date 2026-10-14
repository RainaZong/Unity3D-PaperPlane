# Paper Plane — Physics-Based 2D Gameplay Prototype

## Overview

**Paper Plane** is a 2D physics-based gameplay prototype developed in Unity using C#. The player controls a paper plane's pitch to influence its flight path, while gravity, environmental tailwind, lift, drag, and stall mechanics determine its movement.

The goal was to create a responsive and enjoyable gliding experience by combining physics-inspired simulation, trigonometry, and iterative gameplay tuning.

## Gameplay Preview

<!-- Replace the placeholder below with a 5–10 second gameplay GIF. -->
<!-- Recommended: show the plane launching, pitching up/down, and gliding or stalling. -->

**Gameplay GIF**

https://github.com/user-attachments/assets/7861068e-79b8-4b7b-b0ec-9a77f4f18f87



### Screenshots

**1. Launch Preparation**

<img width="1638" height="850" alt="image" src="https://github.com/user-attachments/assets/1a3fdd18-03ac-40e0-95c5-f23d1f80dd98" />



**2. Flight and Player Controls**

<img width="1626" height="824" alt="image" src="https://github.com/user-attachments/assets/3de7b809-ffee-4be3-99b3-2686a706fab2" />



**3. Game End State**

<img width="1634" height="840" alt="image" src="https://github.com/user-attachments/assets/04e28cee-0cb5-4671-927f-c314e6d0f1f8" />


## Key Features

- **Physics-Based Flight:** Uses an initial launch impulse, gravity, environmental tailwind, aerodynamic lift, and drag to simulate gliding.
- **Player-Controlled Pitch:** Allows the player to pitch the plane up or down using keyboard input, influencing its orientation and flight behavior.
- **Stall Mechanics:** Detects excessive angle of attack combined with low speed, reducing lift and applying an additional downward force.
- **Flight Direction Alignment:** Uses velocity-based orientation with smooth automatic alignment when the player is not actively controlling the plane.
- **Game State Management:** Implements a simple state machine with three states: `Ready`, `Playing`, and `Ended`.
- **Real-Time Flight Feedback:** Displays flight height, horizontal travel distance, and speed during gameplay.
- **Designer-Friendly Tuning:** Exposes core gameplay parameters in the Unity Inspector for rapid iteration without modifying the code.

## Technical Highlights

### 1. Physics and Trigonometry

The flight model combines several physics-inspired forces:

- **Initial Impulse:** Applies the launch force using `Rigidbody2D.AddForce()` with `ForceMode2D.Impulse`.
- **Gravity:** Uses Unity's 2D physics system, with the gravity scale configured through `Rigidbody2D`.
- **Tailwind:** Applies a continuous horizontal environmental force to sustain forward movement.
- **Lift and Drag:** Calculates simplified aerodynamic forces based on the plane's velocity and flight direction.
- **Angle of Attack (AOA):** Uses vector-angle calculations to compare the plane's facing direction with its velocity direction.
- **Trigonometry:** Uses cosine to scale lift, `Mathf.Atan2()` to calculate the velocity direction angle, and degree-to-radian conversion for trigonometric calculations.

The aerodynamic model is intentionally simplified and tuned for gameplay rather than intended to be a full real-world flight simulation.

### 2. Stall and Control Feel

The stall system combines two conditions: the angle of attack exceeds a configurable threshold, and the plane's speed falls below a configurable threshold. When a stall occurs, lift is reduced and an additional downward force is applied.

The prototype also explores the balance between automatic flight-direction alignment and player control. Player input takes priority during active pitch control, while automatic alignment resumes when the player releases the controls.

### 3. State Machine

The game flow is separated into three states:

| State | Behavior |
|---|---|
| `Ready` | Allows the player to adjust the initial launch angle and start the game. |
| `Playing` | Simulates flight, processes player input, and updates flight information. |
| `Ended` | Displays the final flight distance and allows the player to retry. |

This separation keeps launch preparation, active gameplay, and the end state distinct.

## Controls

| Input | Action |
|---|---|
| `W` / `Up Arrow` | Increase the launch angle before starting; pitch up during flight. |
| `S` / `Down Arrow` | Decrease the launch angle before starting; pitch down during flight. |
| `Space` | Start the flight. |
| `R` | Retry after the game ends. |

## Built With

- **Unity** — 2D game development and physics simulation
- **C#** — Gameplay programming
- **Unity Input System** — Keyboard input handling
- **Rigidbody2D** — Physics-based movement and force application
- **Collider2D / PolygonCollider2D** — Collision detection and plane geometry

## Running the Project

1. Clone or download this repository.
2. Open the project using the Unity Editor version specified below.
3. Open the main gameplay scene.
4. Enter Play Mode to test the prototype.

### Environment

- **Engine:** Unity `6000.3.15f1 LTS`
- **Language:** C#
- **Rendering:** 2D
- **Input:** Unity Input System
- **Physics:** Unity 2D Physics

## Project Focus

This prototype was developed as a gameplay design and programming exercise, focusing on:

- Applying physics and trigonometry to gameplay mechanics.
- Designing intuitive and responsive player controls.
- Balancing physically inspired behavior with gameplay feel.
- Iterating on gameplay parameters through playtesting.

