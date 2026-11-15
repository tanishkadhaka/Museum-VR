# Heritage VR museum scene

The project at `/Users/aasthathakur/classwork/ARVR/VR` now contains a complete editable scene:

`Assets/HeritageVR/HeritageMuseum.unity`

Open this scene in Unity 2022.3.62f3 and press Play. It is also the enabled scene in Build Settings. The original SampleScene is preserved.

## Included

- Evening museum entrance with rectangular columns, triangular pediment, warm windows, steps and courtyard.
- Floating Heritage VR menu with Start, Guided Tour and Controls & Comfort. Guided Tour opens the first exhibit's information panel.
- Sacred bronze gallery with procedural Nataraja placeholder, black plinth, brass posts, red ropes and spotlights.
- Exhibit information panel and close inspection with rotation.
- Contemporary ceramics room, ceramic inspection and teleport target.
- URP materials, real-time lighting, headset vignette and input hints.
- OpenXR loader settings for Standalone and Android, Touch and simple controller profiles, and Android Meta Quest feature.

## Desktop controls

Click buttons to select. Press Enter to start or examine. Use WASD to move and hold the right mouse button to look around. Press I in the first gallery to enter inspection, then A/D to rotate. G switches galleries. Escape returns from inspection or goes to the entrance.

## Headset controls

Right controller ray + trigger selects. Right stick snap-turns by 30 degrees; while inspecting it rotates the exhibit. Push the left stick forward for a discrete 1.25-metre step. B returns. The camera uses floor tracking when the runtime supports it.

## Validation and limits

Unity batch compilation and Play Mode flow validation passed. Four previews were rendered by Unity itself, not generated as background pictures. Physical headset operation and an Android build have not been tested. macOS is the desktop preview environment; headset testing needs a whysupported OpenXR runtime/device. Configure the target device's Android build settings and run Unity's XR Project Validation before creating a headset build.

The statue is a simplified procedural teaching placeholder, not a museum-quality scan or historically exact reconstruction. Lighting is real-time, not baked. No external model or photo assets were supplied or downloaded.

## Editing

All objects are saved in the scene hierarchy and can be edited directly. Source files are under `Assets/HeritageVR/Scripts`; the scene generator is under `Assets/HeritageVR/Editor`. The **Heritage VR > Build Museum Scene** command rebuilds the generated scene and replaces manual edits in that scene, so duplicate it before using that command after making changes.

## Desktop repair

The macOS preview now disables mock XR startup, uses an eye-height desktop camera, and renders the Game view at 1x with low-resolution scaling disabled. Legacy text was replaced by TextMeshPro SDF. Inspection hides the exhibit prompt and frames the complete statue. Pointer hit testing was verified in the open editor using camera screen coordinates, and Enter / I were tested through native keyboard input.

The scene is currently running at the entrance. Click Start or focus the Game view and press Enter.
