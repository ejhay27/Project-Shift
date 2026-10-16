# PROJECT: SHIFT — Repository and Environment Audit

Audit date: 2026-10-16 (UTC)
Branch: `arena/0aa5796b-project-shift` (branched from `main` @ `4050465`)

## 1. Repository contents

| Item | Finding |
|---|---|
| Files tracked | 1 file: `README.md` (contents: `# Project-Shift`) |
| Commits | 1 (`Initial commit`, 4050465) |
| Engine / project | **None.** No Unity project (`ProjectSettings/`, `Assets/`, `Packages/`), no Godot, Unreal, Gradle, or Android project files. |
| Source code | None. |
| Assets | None. |
| Dependencies | None (no `package.json`, `.csproj`, `build.gradle`, `Packages/manifest.json`). |
| Build status | Nothing to build. No CI configuration (`.github/workflows` absent). |
| Existing working features | **None.** There is no playable code to preserve. |
| Git | Working tree clean; remote `origin` = `https://github.com/ejhay27/Project-Shift.git`. |

**Implication:** There is no existing engine choice, no migration cost, and nothing to break. Milestone 0 starts from a blank repository, so the decisions below are free to make now and cheap to change later.

## 2. Local toolchain status (this sandbox)

| Tool | Status |
|---|---|
| Unity Editor / Unity Hub | **Not installed.** `download.unity3d.com` and `public-cdn.cloud.unity3d.com` are unreachable (no network). |
| Java / JDK | **Not installed** (`JAVA_HOME` unset). No `api.adoptium.net` or apt mirror access. |
| Android SDK / platform-tools / build-tools / NDK | **Not installed.** `ANDROID_HOME` unset. `dl.google.com` unreachable. |
| Gradle | Not installed. `services.gradle.org` unreachable. Unity bundles its own Gradle, so this is only needed if the Android project is exported and built outside Unity. |
| .NET SDK | Not installed (not needed for Unity; useful only for standalone C# unit tests). |
| Node.js | Present (`/usr/local/bin/node`), yarn 1.22.22. |
| Python 3 | Present (`/usr/bin/python3`). |
| git / gh | Present and authenticated as `ejhay27`. |
| Hardware | 2 vCPU, 3.8 GiB RAM, 20 GB free disk, no swap, x86_64. |
| Android device | None attached. No `adb`. |

**Reachable hosts:** `github.com`, `api.github.com`, `codeload.github.com`, `registry.npmjs.org`, `pypi.org`, `files.pythonhosted.org`.
**Unreachable:** Google Android SDK, Gradle distribution, Unity download/CDN, JDK mirrors, Ubuntu archives.

**Consequence:** In this sandbox the following are impossible today:
- Building a Unity project or Android APK.
- Compiling or running C# gameplay code (Unity tests need the Unity editor or a Unity-compatible test runner).
- Running the Android Gradle build.
- Profiling on a device.

Every milestone exit condition that needs an APK, device run, or Unity build is therefore **Blocked** until one of the following is provided (see `docs/02_ROADMAP.md`, section "Unblocking options"):
1. Allowlist additions for `dl.google.com`, `services.gradle.org`, `download.unity3d.com`, `public-cdn.cloud.unity3d.com`, and a JDK mirror; or
2. The owner builds on a local machine or self-hosted CI runner and commits the results/logs; or
3. A pre-provisioned image with Unity 6, JDK 17, and the Android SDK.

## 3. Requirements the audit checked against the environment

| Requirement (from spec) | Current status |
|---|---|
| Inspect repo before change | Done (this file). |
| Confirm engine, SDK, JDK, build tools, rendering pipeline, networking | Partially done. Engine and networking are decided on paper (see `01_ARCHITECTURE_DECISION.md`). SDK/JDK/build-tools versions are documented from published Unity guidance but **not verified by a build**. |
| Recoverable version control | Git repository in place; this milestone is committed to the working branch. |
| Android build pipeline demonstrated | **Blocked** (toolchain unreachable). |

## 4. Facts used for the Android toolchain plan

Verified from public sources during the audit:
- Unity's Android environment requires the Android Build Support module, the Android SDK, the NDK, and a JDK. Each Unity version pins specific NDK and JDK versions, and Unity 6 uses JDK 17 with Gradle 8.x (Unity forum and Unity manual, 2023.2 docs).
- Android Gradle Plugin 8.1+ requires JDK 17 (Google AdMob Unity build guide).
- Google Play policy: from **2026-08-31**, new apps and app updates must target **Android 16 (API 36)** or higher; existing apps must target at least API 35 to remain visible to new users on newer Android versions (median.co and foresightmobile.com summaries of Google's policy, 2026). **Verify against Google's official Target API level page before release**; third-party summaries may lag.

These are planning inputs. The exact Unity 6 version (6000.x) and matching NDK/JDK must be read from the installed Unity editor's Android Build Support settings when the toolchain is available.

## 5. Risks identified at audit

| # | Risk | Severity | Mitigation |
|---|---|---|---|
| R1 | No Android build or device test possible in the current sandbox. | High | Unblocking options in roadmap; keep all unverified claims marked Blocked. |
| R2 | Spec scope (≈37 system groups) far exceeds any single milestone. | High | Milestone-by-milestone delivery; registry tracks each requirement. |
| R3 | Unity 6 Android toolchain version coupling (NDK/JDK/Gradle pinned per editor version). | Medium | Pin editor version in `ProjectSettings/ProjectVersion.txt` once created; record SDK/NDK/JDK in `docs/`. |
| R4 | Local multiplayer on hotspots: client isolation, Android network restrictions, multicast/broadcast filtering. | High | Prototype discovery + manual IP path early (Milestone 8 spike, earlier if feasible). |
| R5 | Mobile thermal/battery limits under open-world + AI + particles. | High | Quality presets, simulation tiers, profiling gate per milestone. |
| R6 | Save corruption on app interruption. | High | Atomic write-then-rename, backups, versioned schema (Milestone 1). |
| R7 | Exclusivity rules (World Tree Guardian, cooldowns, rewards) are exploit-prone in multiplayer. | High | Host-authoritative state, persisted outcome flags, tests listed in registry. |
| R8 | Asset licensing: spec references inspiration titles. | Medium | Only original or properly licensed assets; no copied characters/UI. |
| R9 | Store target API deadline (API 36 for new/updated apps from 2026-08-31). | Medium | Set target API to 36 in the Android build settings when the project is created. |
| R10 | Generated voice lines and AI assets need quality and licensing review. | Medium | Pre-render and store audio; keep a manifest with provenance. |
| R11 | Real-world weapon construction content must not be produced. | Policy | Ammunition and weapons stay fictional; no real-world build instructions. |

## 6. Audit conclusion

The repository is a blank scaffold. There is nothing to preserve, and the engine decision is free. The sandbox cannot produce or test an Android build, so Milestone 0's exit condition is **not met** and is recorded as Blocked. All planning artifacts in `docs/` are produced now, so work can resume immediately once a toolchain is available.
