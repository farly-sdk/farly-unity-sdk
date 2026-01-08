# Farly SDK for Unity

## Documentation
The full documentation can be found at https://mobsuccess-group.notion.site/Farly-Unity-SDK-2e2d6183107581239214e5aa84fae8dc

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

## Example project
An example project is available at https://github.com/farly-sdk/farly-unity-sdk-example

# Contributing

## IDE(s):
- Unity HUB (and Unity editor)
- VSCode with extensions `ms-dotnettools.csdevkit`
- .net installed (vscode restart needed)
- xcode

## Making changes

Dependencies are handled via a dependencies file at [`./Assets/Editor/Dependencies.xml`](./Assets/Editor/FarlyDependencies.xml)

The Unity code is at [`./Assets/Plugins/FarlySDK.cs`](./Assets/Plugins/FarlySDK.cs). This file exposes our methods and is the entry point for the SDK - it calls the native code.

Native bridges are under [`./Assets/Plugins/Android`](./Assets/Plugins/Android) and [`./Assets/Plugins/iOS`](./Assets/Plugins/iOS), those files make the bridge between the native code and the unity code.

The best way to work on the SDK is to use the example project at https://github.com/farly-sdk/farly-unity-sdk-example. By default this project uses the package from the git url, but you can change it to use the local version of the package by removing the git url from the manifest and adding a local path to the package.

Then, you will be able to launch the project on Android/iOS and test your changes.
To launch the project:
- Open the project in Unity
- File/Build Settings
- Select the platform you want to test and click "Switch Platform" if needed
- Click "Build and Run"
- On iOS, this will open XCode, you can then run the project on a simulator or a device - we recommend the device as the simulator never get offers
- On Android, this will open Android Studio, you can then run the project on a simulator or a device - we recommend the device as the simulator never get offers

## Deploying a new version
- Update the version in [`./package.json`](./package.json)
- Push to master
