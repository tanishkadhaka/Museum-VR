# Heritage VR — Meta Quest standalone

Target headsets: Meta Quest 2, Quest 3 and Quest 3S, running directly on the headset.

Project: `/Users/aasthathakur/classwork/ARVR/VR-Virtual-Museum-main`
Scene: `Assets/Scenes/UnifiedMuseum.unity`

## Quest controls

Aim the right-controller ray and press Trigger to select Start, exhibits, and menu buttons. Right stick snap-turns by 30 degrees; during inspection it rotates the artifact. Push the left stick forward and release it for a collision-aware step. B returns from inspection or returns to the entrance. Room-scale head tracking updates before rendering. No hand-tracking interaction is configured; use the controllers.

## Technical target

Unity 2022.3.62f3, OpenXR 1.17.1, Meta Quest support, Touch/Touch Plus controller profiles, Android ARM64, IL2CPP, OpenGL ES 3, single-pass stereo, linear color, 4x MSAA, ASTC texture overrides. Android minimum API 29, target API 34. Quest 2, Quest 3 and Quest 3S device entries enabled. Desktop preview remains available on macOS.

Android Build Support, SDK, NDK and OpenJDK were installed through Unity Hub. The project uses the new Input System for keyboard/mouse preview, with XR input for the headset and controllers.

## Build and installation

The Unity menu `Museum > 09 Build Quest APK` builds `HeritageVR-Quest.apk` into this chat's outputs directory. The APK build succeeded with zero errors. Its manifest declares Quest 2/3/3S, required VR head tracking and the VR launch category. ARM64 IL2CPP/OpenXR libraries and the paintings were verified inside the APK. No Quest was connected for physical testing. Build status is recorded in `Quest-Build.txt`, and OpenXR configuration validation is recorded in `Quest-Compatibility.txt`.

With a USB-debugging-authorized Quest connected, install the APK using Meta Quest Developer Hub or:

```
adb install -r HeritageVR-Quest.apk
adb shell monkey -p com.heritagevr.virtualmuseum -c android.intent.category.LAUNCHER 1
```

An APK build and static configuration checks cannot establish on-device comfort or performance. The device test should confirm stereoscopic rendering, head tracking, both controllers, Start, exhibit selection, inspection, snap turn, step movement, and Back. Physical testing remains required until those checks have been run on a Quest.

## Paintings

Three original paintings are mounted in brass frames on the gallery's back wall: River of Memory, Vessels & Arches, and Lotus at Dusk. Their labels and frames are grouped under `BACK WALL • Painting collection`.

Project texture: `Assets/HeritageVR/Paintings/HeritageTriptych.png`. Separate materials crop each third of this atlas. An export copy is supplied as `HeritageTriptych.png` alongside this guide.

Generation used the built-in image-generation tool. Final prompt:

> Create a single wide museum painting triptych texture atlas, aspect ratio 3:1. Three equally wide square paintings aligned edge to edge with no gaps, no frames, no wall, no labels, no signature, flat frontal view suitable for a Unity texture. An original fine-art series in warm ochre, muted teal, indigo and ivory with visible painterly brushstrokes. Left square: atmospheric river and ancient sandstone temple landscape at dawn. Middle square: lyrical abstract composition of terracotta vessels, circles and architectural arches. Right square: serene lotus pond with distant blue hills at twilight. Each square is a complete independent composition. Elegant cohesive artwork for a university virtual heritage museum. High detail, clean edges, no text.
