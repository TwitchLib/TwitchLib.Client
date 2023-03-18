# Prerequisites to execute/run tests (actually)

- check out https://github.com/TwitchLib/TwitchLib.Communication/pull/17
- change TwitchLib.Client/TwitchLib.Client.csproj
    - remove PackageReference to TwitchLib.Communication
    - add ProjectReference to the checked out TwitchLib.Communication-Project