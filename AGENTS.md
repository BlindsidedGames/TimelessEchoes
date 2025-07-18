# Repository Instructions

When making changes to this **2D** Unity project:

- Consult the official Unity documentation at <https://docs.unity3d.com> to confirm APIs and best practices.
- Ensure compatibility with **Unity 6000.1.6f1**, the version used by Timeless Echoes.
- Do not create or commit `.meta` files unless absolutely required.
- Avoid using obsolete Unity API calls. For example, replace `Object.FindObjectOfType` with `Object.FindFirstObjectByType` or `Object.FindAnyObjectByType`.
- Note the warning `CS0618: 'CinemachineVirtualCamera' is obsolete`. Use `CinemachineCamera` instead of the deprecated `CinemachineVirtualCamera`.
- Do not modify `Assets/Scenes/Main.unity` unless explicitly instructed to do so.
