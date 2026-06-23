# 🥊 Street Fighter — C# WinForms OOP

> A fully-featured two-player Street Fighter–inspired fighting game built in **C# Windows Forms (.NET Framework 4.7.2)**, with a clean OOP architecture across layered Controllers, Managers, Models, and Interfaces — no game engine required.

![Language](https://img.shields.io/badge/Language-C%23-239120?style=flat-square&logo=csharp)
![Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4?style=flat-square&logo=dotnet)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?style=flat-square&logo=windows)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

---

## 📖 Overview

A Street Fighter II–inspired 2D fighting game built entirely with C# and Windows Forms as an OOP semester project. It features 8 playable characters, GIF-based sprite animation, a full combat system with hit detection, an AI opponent, a VS (2-player) mode, a HUD with custom fonts, in-game music, and a fully remappable key bindings system — all without Unity, Godot, or any third-party game framework.

---

## ✨ Features

- 🎮 **Single Player (vs AI) & VS Mode (local 2-player)** — both selectable from the main menu
- 🧍 **8 Playable Characters** — Ryu, Ken, Chun-Li, Blanka, Guile, Zangief, E. Honda, Dhalsim
- ⚡ **Character-Specific Specials** — Ryu's Hadouken fireball, Ken's Hurricane Kick, Chun-Li's Spinning Bird Kick, Blanka's full-screen Electric Thunder, Guile's Sonic Boom, Zangief's Spinning Lariat, E. Honda's Hundred Hand Slap, Dhalsim's Yoga Fire
- 🥊 **Full Combat System** — light punch, heavy punch, light kick, heavy kick, uppercut, crouch kick, and special moves; each with unique hit distances and damage values
- 💥 **Hit Detection & Knockback** — rectangle-based attack boxes, hit stun timers, invincibility frames, and positional knockback
- 🤖 **State-Machine AI** — 4-state AI (approach, retreat, strafe, wait) with distance-aware attack decisions and cooldowns
- ❤️ **Health Bars & Timer** — 100 HP per player, 60-second countdown; KO or time-out determines the winner
- 🖥️ **HUD with Custom Fonts** — "Fighting Spirit 2" font renders player names, health bars, and the match timer
- 🎵 **Stage Music & SFX** — looping background music and KO/draw sound effects via WMPLib
- 🗺️ **Animated Stage Backgrounds** — 12 authentic SF2 stage GIFs (Ryu, Ken, Guile, Dhalsim, Zangief, E. Honda, Sagat, Bison, Balrog, and more)
- 🎬 **VS Splash Screen** — character portrait reveal screen shown before every fight
- 🗂️ **Character Select Screen** — grid-based navigator for both single-player and VS mode with yellow/green selection cursors and looping music
- ⚙️ **Fully Remappable Controls** — Options screen lets players rebind every key via a DataGridView UI; bindings persist to `Assets/Config/keybindings.txt`
- 🔁 **Custom GIF Animation Engine** — `GifSpeedController` handles per-character GIF playback speed, horizontal mirroring, and frame callbacks

---

## 🏗️ Project Structure

```
STREET-FIGHTER-WINFORMS-PROJECT/
│
├── GAME PROJECT/
│   │
│   ├── Animations/                     # Character class hierarchy
│   │   ├── BaseCharacter.cs            # Abstract base: animations, hit reactions, GIF management
│   │   ├── Ryu.cs                      # Special: Hadouken (fireball projectile)
│   │   ├── Ken.cs                      # Special: Hurricane Kick
│   │   ├── ChunLi.cs                   # Special: Spinning Bird Kick
│   │   ├── Blanka.cs                   # Special: Electric Thunder (full-screen)
│   │   ├── Guile.cs                    # Special: Sonic Boom
│   │   ├── Zangief.cs                  # Special: Spinning Lariat
│   │   ├── E_Honda.cs                  # Special: Hundred Hand Slap
│   │   ├── Dhalsim.cs                  # Special: Yoga Fire
│   │   └── GifSpeedController.cs       # Custom GIF frame extractor, speed scaler & mirror
│   │
│   ├── Controllers/
│   │   ├── InputController.cs          # Maps raw Keys → PlayerInputState (IInputHandler)
│   │   ├── MovementController.cs       # Moves characters, boundary clamping & overlap rules
│   │   ├── CombatController.cs         # Executes attacks, hit detection, revert timers
│   │   └── AIController.cs             # 4-state AI machine: approach/retreat/strafe/attack
│   │
│   ├── Managers/
│   │   ├── MatchStateManager.cs        # Health, countdown timer, KO detection (Observer)
│   │   ├── HUDRenderer.cs              # Health bars, timer, names via custom fonts (IRenderable)
│   │   ├── SoundManager.cs             # Stage music & result sounds via WMPLib
│   │   ├── StageManager.cs             # Animated stage background management
│   │   ├── ProjectileManager.cs        # Fireball spawning and movement
│   │   └── CharacterFactory.cs         # Factory: instantiates BaseCharacter subclass by name
│   │
│   ├── Interfaces/
│   │   ├── IInputHandler.cs            # HandleKeyDown / HandleKeyUp
│   │   ├── IMatchObserver.cs           # OnMatchOver(winnerName, isDraw)
│   │   └── IRenderable.cs              # Render(Graphics, canvasWidth, canvasHeight)
│   │
│   ├── Models/
│   │   ├── MatchState.cs               # Health, time, IsOver, result text — data only
│   │   └── PlayerInputState.cs         # Per-player key press flags + attack lock state
│   │
│   ├── Assets/
│   │   ├── Audio/
│   │   │   ├── MainMenuTheme.wav        # Main menu looping music
│   │   │   ├── Ryu-SFA2-Gold-Theme.wav  # Character select screen music
│   │   │   ├── stageMusic.mp3           # In-fight looping stage music
│   │   │   └── ko.mp3                   # KO / draw result sound
│   │   ├── Characters/
│   │   │   ├── Ryu/                     # Standing, Forward, Backward, LeftPunch, RightPunch,
│   │   │   ├── Ken/                     # HighKick-Left/Right, UpperCut-Left/Right,
│   │   │   ├── Chun-Li/                 # crouch-left/right, and special GIFs
│   │   │   ├── Blanka/                  # (electricity.gif)
│   │   │   ├── Guile/                   # (sonicboom.gif)
│   │   │   ├── Zangief/                 # (spinning.gif)
│   │   │   ├── E. Honda/                # (handSlap.gif)
│   │   │   └── Dhalsim/                 # (yogafire.gif)
│   │   ├── Stages/                      # 12 animated SF2 stage GIFs
│   │   │   ├── sf2-ryu-stage-animated.gif
│   │   │   ├── sf2-ken-stage-animated.gif
│   │   │   ├── sf2-ehonda-stage-animated.gif
│   │   │   ├── streetfighter2-guile-stage.gif
│   │   │   └── ...
│   │   ├── Fonts/
│   │   │   ├── Fighting Spirit 2.otf
│   │   │   ├── Fighting Spirit 2 bold.otf
│   │   │   └── Fighting Spirit 2 ital.otf
│   │   ├── UI/
│   │   │   ├── Main Menu.png
│   │   │   ├── select_screen.png
│   │   │   ├── vs_screen.gif
│   │   │   ├── ko.png
│   │   │   ├── CoinPage.png
│   │   │   └── vsScreenCharacterImages/  # Portrait PNGs for all 8 characters
│   │   └── Config/
│   │       └── keybindings.txt           # Persisted key bindings (auto-generated)
│   │
│   ├── FightScreen.cs                  # Main game loop (Timer-driven), wires all components
│   ├── MainMenu.cs                     # Arrow-based menu: Start / VS / Options / Quit
│   ├── CharacterSelectScreen.cs        # P1 character selection grid
│   ├── VsCharacterSelectScreen.cs      # Extends CharacterSelectScreen for two-pass P1+P2 selection
│   ├── Vs_Screen.cs                    # VS portrait splash before the fight
│   ├── OptionsScreen.cs                # Key rebinding UI (DataGridView + KeyPressDialog)
│   ├── KeyBindings.cs                  # Serialises/deserialises all bindings to Config/
│   ├── AnimatedGif.cs                  # WinForms GIF playback helper
│   ├── MainWindow.cs                   # Shell form: hosts all child screens in a panel
│   ├── Program.cs                      # Entry point — Application.Run(new MainWindow())
│   └── STREET FIGHTER.csproj
│
└── STREET FIGHTER.slnx                 # Visual Studio 2022 solution file
```

---

## 🧠 OOP & Design Patterns

| Principle / Pattern | Where it's applied |
|---|---|
| **Single Responsibility** | Each Controller and Manager has exactly one job |
| **Open/Closed** | Add a character by subclassing `BaseCharacter` and adding one `case` in `CharacterFactory` — `FightScreen` unchanged |
| **Liskov Substitution** | Every character IS-A `BaseCharacter` and is fully substitutable wherever one is expected |
| **Interface Segregation** | `IInputHandler`, `IMatchObserver`, `IRenderable` — consumers depend only on what they use |
| **Dependency Inversion** | `CombatController` receives `MatchStateManager` and `ProjectileManager` via constructor injection |
| **Factory Pattern** | `CharacterFactory.Create(name, pos, mirrored)` centralises and decouples object creation |
| **Observer Pattern** | `MatchStateManager` notifies `FightScreen` via `IMatchObserver.OnMatchOver` |
| **State Machine** | `AIController` runs a 4-state machine with distance-based transitions and attack cooldowns |
| **Template Method** | `CharacterSelectScreen` defines the selection flow; `VsCharacterSelectScreen` overrides steps for two players |

---

## 🛠️ Tech Stack

| Component | Technology |
|---|---|
| Language | C# |
| UI Framework | Windows Forms (WinForms) |
| Target Framework | .NET Framework 4.7.2 |
| IDE | Visual Studio 2022 |
| Solution Format | `.slnx` (VS 2022 17.10+) |
| Animation | Custom `GifSpeedController` (GDI+) |
| Audio | `System.Media.SoundPlayer` + WMPLib (COM interop) |
| Fonts | GDI+ `PrivateFontCollection` — "Fighting Spirit 2" |
| Projectiles | `ProjectileManager` driven by `System.Windows.Forms.Timer` |

---

## 🚀 Getting Started

### Prerequisites

- Windows OS
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (Community or higher)
  - Workload: **.NET desktop development**
- .NET Framework 4.7.2 (included in VS 2022 by default)

### Run

```bash
git clone https://github.com/its-abuzar/STREET-FIGHTER-WINFORMS-PROJECT.git
```

Open `STREET FIGHTER.slnx` in Visual Studio 2022 and press **F5**.

> Assets are loaded from the `Assets/` folder relative to the executable. When running via Visual Studio, the build copies assets to `bin/Debug/` automatically. If running the `.exe` directly, ensure the `Assets/` folder is in the same directory.

---

## 🎮 Default Controls

| Action | Player 1 | Player 2 |
|---|---|---|
| Move Left | `←` | `G` |
| Move Right | `→` | `J` |
| Move Up | `↑` | `Y` |
| Move Down | `↓` | `H` |
| Light Punch | `A` | `B` |
| Heavy Punch | `S` | `N` |
| Light Kick | `Z` | `M` |
| Heavy Kick | `X` | `,` |
| Special | `D` | `.` |

> All controls are remappable in-game via **Main Menu → OPTION**. Bindings are saved to `Assets/Config/keybindings.txt` and loaded automatically on startup.

**Modifier combos:** hold `↑` while pressing a punch key for an **Uppercut**; hold `↓` while pressing a kick key for a **Crouch Kick**.

---

## 🧍 Characters & Specials

| Character | Special Move | Animation File |
|---|---|---|
| Ryu | Hadouken (fireball projectile) | `Ryu-fireballs.gif` |
| Ken | Hurricane Kick | `ken-hurricane-loop.gif` |
| Chun-Li | Spinning Bird Kick | `Chun-Li-spinningbird.gif` |
| Blanka | Electric Thunder (full-screen) | `Blanka-electricity.gif` |
| Guile | Sonic Boom | `Guile-sonicboom.gif` |
| Zangief | Spinning Lariat | `Zangief-spinning.gif` |
| E. Honda | Hundred Hand Slap | `E. Honda-handSlap.gif` |
| Dhalsim | Yoga Fire | `Dhalsim-yogafire.gif` |

---
## 📸 Screenshots

| Main Menu | Character Select |
|---|---|
| ![Main Menu](./GAME%20PROJECT/Project%20Screenshots/MAIN%20MENU.png) | ![Character Select](./GAME%20PROJECT/Project%20Screenshots/CHARACTER_SELECT.png) |

| VS Screen | Fight Screen |
|---|---|
| ![VS Screen](./GAME%20PROJECT/Project%20Screenshots/VS_SCREEN.png) | ![Fight Screen](./GAME%20PROJECT/Project%20Screenshots/FIGHT_SCREEN.png) |

| Options Mode |
|---|
| ![Options Mode](./GAME%20PROJECT/Project%20Screenshots/OPTIONS_MODE.png) |

> All screenshots are from the actual game running in Windows Forms.

---

## 📄 License

MIT — see [LICENSE.txt](LICENSE.txt).

---

## 👤 Author

**Abuzar** · [@its-abuzar](https://github.com/its-abuzar)

---

> *No game engine. Just C#, WinForms, and a state machine that knows when to throw a fireball.* 🕹️
