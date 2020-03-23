## MonoGame IME Helper for desktop and mobile platforms

### Targeted MonoGame platforms

- MonoGame.Framework.WindowsDX
- MonoGame.Framework.DesktopGL
- MonoGame.Framework.Android
- MonoGame.Framework.iOS

## Getting started

### NuGet

```
PM> Install-Package MonoGame.IMEHelper.WindowsDX
```

All available packages for specific platforms:

```
MonoGame.IMEHelper.WindowsDX
MonoGame.IMEHelper.DesktopGL
MonoGame.IMEHelper.Android
MonoGame.IMEHelper.iOS
MonoGame.IMEHelper.Common
```

### Initialize a IMEHandler instance in your game Initialize method

```c#
protected override void Initialize()
{
   imeHandler = new WinFormsIMEHandle(this);
   imeHandler.TextInput += (s, e) => { ... };
}
```

### If your want to render Composition String

```#
   imeHandler = new WinFormsIMEHandler(this);
   imeHandler.TextInput += (s, e) => { ... };
   imeHandler.TextComposition += (s, e) => { ... };
```

*Note that `TextComposition` event only works on WindowsDX platform due to limitation of the underlying platform*

### MonoGame.IMEHelper.Common package

If you has used a shared netstandard project to share game code. You can reference the `MonoGame.IMEHelper.Common` package. It's just includes API interfaces and no implementation. Then you add the platform specific package to include the platform IME implementation.

### Start Text Composition

`imeHandler.StartTextComposition();`

### Stop Text Composition

`imeHandler.StopTextComposition();`

### Get VirtualKeyboardHeight (only for mobile platforms)

`imeHandler.VirtualKeyboardHeight`

## Android Extra Setup

You have to change your Activity's base class to `AndroidGameActivityIME`, like the following:

```c#
public class Activity1 : AndroidGameActivityIME
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        var g = new Game1();
        SetContentView((View)g.Services.GetService(typeof(View)));
        g.Run();
    }
}
```

## License

MonoGame.IMEHelper is released under the [The MIT License (MIT)](https://github.com/ryancheung/MonoGame.IMEHelper/blob/master/LICENSE.txt).
