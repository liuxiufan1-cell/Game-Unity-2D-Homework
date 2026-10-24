# Game-Unity-2D-Homework

[简体中文](README.md) | [日本語](README.ja.md) | **English**

A Unity 2D platformer coursework project. The repository keeps the playable game, reusable development assets, and the project files needed to open and edit it locally in Unity.

## Development environment

| Item | Version or path |
|---|---|
| Unity Editor | `6000.3.13f1` |
| Rendering | URP `17.3.0`, 2D Renderer |
| Input | Input System `1.19.0` |
| Main scene | [2DGameCoursework.unity](Assets/Game/Scenes/2DGameCoursework.unity) |

## Open locally

1. Clone the repository, or select **Code → Download ZIP** on GitHub and extract it.

   ```sh
   git clone https://github.com/liuxiufan1-cell/Game-Unity-2D-Homework.git
   ```

2. Install Unity `6000.3.13f1` through Unity Hub and activate your Editor license.
3. Use **Add project from disk** to add the repository root: the folder containing `Assets`, `Packages`, and `ProjectSettings`.
4. Wait for the initial package resolution, asset import, and script compilation to finish.
5. Open `Assets/Game/Scenes/2DGameCoursework.unity`, press **Play**, and focus the Game window.

To export a player, use **File → Build Profiles**. Install the target platform module and confirm that the main scene is included in the build.

## Gameplay and controls

- Horizontal movement, double jump, and a following camera.
- Walking and jumping enemies, stomp detection, and respawning after death.
- Collectible coins, scoring, goal detection, and a final score display.

| Action | Keyboard | Configured gamepad binding |
|---|---|---|
| Move left or right | `A / D` or `← / →` | Left stick |
| Jump / double jump | `Space` | Bottom face button, such as Xbox A |

Gamepad bindings are documented from the input configuration; physical devices have not been tested. The current player script uses `Move` and `Jump`. Other default Actions in the input asset do not indicate implemented gameplay features. Collecting a coin logs the score to the Console; reaching the goal displays the final score.

## Project layout

| Path | Contents |
|---|---|
| `Assets/Game` | Gameplay scripts, main scene, game prefabs, and animations |
| `Assets/Input` | Input Actions and the generated C# wrapper |
| `Assets/CourseSamples` | Retained terrain, animations, and reusable asset prefabs; old sample code has been removed |
| `Assets/ThirdParty` | External background, character, enemy, coin, and other image assets |
| `Assets/Settings` | URP, 2D Renderer, and rendering settings |
| `Packages`, `ProjectSettings` | Unity dependencies and project configuration |
| `Docs/Changelog` | Change history |
| `Docs/Reviews` | Review records and validation scope |

Development assets are retained. Restored asset prefabs have had components and animation callbacks that depended on deleted sample code removed. Attach the gameplay scripts you need when reusing them. These prefabs are not all complete, independently working characters or managers.

Move or rename assets through Unity's Project window and keep their `.meta` files to preserve GUID references. Generated folders such as `Library` and `Temp` do not need to be committed.

## Validation and records

The latest gameplay validation covers `ed937e1`. Unity import and compilation, script/reference checks for all 14 prefabs and the main scene, and a basic gameplay smoke test passed. Checks ran headlessly on Linux. The Windows Editor GUI, rendered output, physical input devices, and exported players have not been tested. A pre-existing Unity Search indexing exception remains in the cloud logs.

- [Changelog (Chinese)](Docs/Changelog/CHANGELOG.md)
- [2026-10-24 review (Chinese)](Docs/Reviews/2026-10-24.md)
- [Third-party copyright and usage notices](THIRD_PARTY_NOTICES.txt)
