# Farly SDK for Unity

## Getting Started
Add the package by using Unity's package manager.
Either using the editor's interface, and adding a git url to `https://github.com/farly-sdk/farly-unity-sdk.git`, or by adding the following entry to your `Packages/manifest.xml`:
```json
{
  "dependencies": {
    "com.farly.farly-unity-sdk": "https://github.com/farly-sdk/farly-unity-sdk.git",
  }
}
```

> [!IMPORTANT]
> Dependencies are handled via a dependencies file at [`./Assets/Editor/Dependencies.xml`](./Assets/Editor/FarlyDependencies.xml)

This package uses the [unity jar resolver](https://github.com/googlesamples/unity-jar-resolver?tab=readme-ov-file) to resolve dependencies. If you are using other packages that use the jar resolver, you may need to include the dependencies in your own way.

## Documentation
The full documentation can be found at https://mobsuccess.notion.site/Farly-Unity-SDK-9211e505ff7442e4be431db729c6460b?pvs=4

## Example project
An example project is available at https://github.com/farly-sdk/farly-unity-sdk-example

# Contributing

## IDE(s):
- Unity HUB (and Unity editor)
- VSCode with extensions `ms-dotnettools.csdevkit`
- .net installed (vscode restart needed)

## Making changes

Dependencies are handled via a dependencies file at [`./Assets/Editor/Dependencies.xml`](./Assets/Editor/FarlyDependencies.xml)

The Unity code is at [`./Assets/Plugins/FarlySDK.cs`](./Assets/Plugins/FarlySDK.cs). This file exposes our methods and is the entry point for the SDK - it calls the native code.

Native bridges are under [`./Assets/Plugins/Android`](./Assets/Plugins/Android) and [`./Assets/Plugins/iOS`](./Assets/Plugins/iOS), those files make the bridge between the native code and the unity code.

## Deploying a new version
- Update the version in [`./package.json`](./package.json)
- Push to master
