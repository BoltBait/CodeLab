# How to Compile CodeLab

#### Before You Begin

- Install the latest version of [Paint.NET](https://www.getpaint.net/). Make sure you use the classic installer. Installing Paint.NET from the Microsoft Store will not work for CodeLab development.

- Install [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) (C# options). The free Community Edition works fine.

#### Debugging CodeLab

While developing CodeLab, it is recommended to use the `FastDebug` configuration. This skips the ILMerge operation, and allows CodeLab to run outside of Paint.NET.

#### Build the Release Version of CodeLab

1. Click the menu: Build > Rebuild Solution

1. If the build fails, you'll need to debug that first.

1. If the build succeeds, a window will open and you'll see `CodeLab.dll` and `Install_CodeLab.bat`
