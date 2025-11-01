# Re-Run After Interruption (Better Runner)

Enhanced Shift toggle mode: The running state will now automatically resume after being interrupted by firing, aiming, or eating. During a run, you can now fire your weapon (this temporarily switches you to walking mode). 

- Holding Shift to run remains unchanged.
- The Little Finger Protection Association calls on you to care about the health of your little finger!

## 🔗 Steam Workshop Link
Click here → [Re-Run After Interruption(Better Runner)](https://steamcommunity.com/sharedfiles/filedetails/?id=3594173614)

## 📥 Download
Click [here](https://github.com/ZhouShuren1881/duckov_mod_better_runner/releases) to download a zip.

## Core Concepts

1. If the user hasn't explicitly changed the running state, restore the `runInputBuffer` value before the next `Update` (the value of `runInputBuffer` may be modified between two `Update` calls)
2. Listen to user mouse input and the character's running state. If the character exits the running state while the user's left mouse button is still pressed, trigger firing.

## C# Version Compatibility

- Target framework: `.NET Standard 2.1`
- Compiled using .NET 8+ [Download .NET - Microsoft](https://dotnet.microsoft.com/en-us/download)

## Build Process
1. Modify the `DuckovPath` variable in the `.csproj` file to point to the actual game directory
2. Copy `0Harmony.dll` from the [Repo Release](https://github.com/ZhouShuren1881/duckov_mod_better_runner/releases) to the project root directory
3. Run the build command:  
   `dotnet build /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary`
