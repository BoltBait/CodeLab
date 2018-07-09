# How to Compile CodeLab

#### Before You Begin

- Install Paint.NET version 4.0.6

- Install Visual Studio 2017 Community Edition (C# options)

- Visit the following page and install ILMerge:

> https://www.microsoft.com/en-us/download/details.aspx?id=17630

#### Add ScintillaNET NuGet Package

- Open the project in Visual Studio

- Click the menu: Tools > NuGet Package Manager > Manage NuGet Packages for Solution...

- Search for "scintillaNET":

> ![Image of NuGet Package Manager](/CONTRIBUTING.png)

- Select the signed package and click the Install button to install it.

#### Build the Release Version of CodeLab

- Click the menu: Build > Rebuild Solution

- If the build fails, you'll need to debug that first.

- If the build succeeds, a window will open and you'll see CodeLab.dll and Install_CodeLab.bat
