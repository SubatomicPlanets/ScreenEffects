# ScreenEffects
ScreenEffects is a simple Unity project that uses Windows API calls to draw effects over the entire screen.  
That's right, apply your post-processing shaders to the entire Windows screen!
3 sample shaders are included, one applies no effects, one increases image contrast, and one adds effects like scanlines to mimic CRT monitors.  
I encourage you to write your own shaders and play around with the code to get the results you want.  
> [!NOTE]
> - Some menus such as the Windows start menu are rendered above the effects meaning they will look like normal.  
> - It was only tested on one device, no idea how this behaves with multiple monitors.

### Requirements:
- Only works on Windows
- Made with Unity 6000.1.2 but since it's just 2 simple scripts and 3 test shaders it should pretty much work on any Unity version

### How to run:
Either clone the project and build an executable or just copy the files needed into your project (they are very simple).
> [!IMPORTANT]
> To exit you have to kill the window in task manager because it won't show in the taskbar!

### How it works:
It basically takes a screenshot, puts it through a shader to apply effects, and displays it in a fullscreen window. It does this in an update loop. The fullscreen window is special since it ignores and passes through all mouse clicks and renders in front of nearly everything else. It is also not captured by the screenshots to avoid weird recursive effects and doesn't show on the taskbar.

### Performance:
Performance was not tested a lot. It has to take a screenshot over 60 times a second and apply a shader to it. This does increase CPU and GPU usage. Depending on the effect shader and machine it's running on, it might not run very well.

### Files:
- WindowManager.cs: Sets window properties like ignoring mouse clicks and always rendering in front
- ScreenCapture.cs: Captures the screen and passes the texture to a shader
- EffectCRT.shader: A shader that applies noise, scanlines, distortion, and other effects to look a bit like a CRT monitor
- EffectContrast.shader: A simple shader that adds a bit of contrast
- EffectNone.shader: A simple shader that applies no effects and is used for testing
