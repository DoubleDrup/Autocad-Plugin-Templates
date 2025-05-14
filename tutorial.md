# Create your personal template for .NET
Starting a new project can take up some unnecessary time. You pick your favorite template from the standard library and start coding, but you realize that you could be copying code from a previous project. A custom template could be a good idea.

I noticed while creating AutoCAD plugins that the starting point of each project is always the same, and I was sometimes copying code from one project to the other. It took me some time to learn about project templates, but it definitely helps speeding up the start of my newest plugin. The other advantage is that you always have the same, if done correctly, clean base upon which you just extend to create your program. So a template makes the starts of your projects faster and more consistent to each other.

This tutorial will focus on making an AutoCAD plugin template package, but whether you are here for the AutoCAD part or just want to understand the templates for .NET, following this tutorial will enable you to create complex templates using basic building blocks.

### Prerequisites
- A basic understanding of programming and using the terminal. This tutorial focusses on a C# AutoCAD plugin, but we will not be doing a lot of programming.
- AutoCAD. Everything we will do also works for the trial version.
- This tutorial uses Rider as a code editor, but feel free to use any IDE. You can even use a text editor with a terminal. 

## Basic Templates
In the first part of this tutorial, we create a very basic yet powerful template. You will get to understand the template.json file and add your first files to the template. Finally, you create a package of your template to save it for yourself or share it with others and along the way you will pick up some tricks that help debugging with AutoCAD a lot more efficient.

### The backbone of a template
There is one key element that all templates need: The `template.json` file. It contains all the configuration data for the implementation of your template. Once this file is added to a `.template.config` folder, .NET will be able to understand that the files and folders that are in the same folder as the .template.config folder belong to a template. Let's create an example template without using any code editor.

[comment]: <> (tree signs: │ ├─── └───)
```
Autocad.Template.Basic
└───.template.config 
    │   template.json
```

1. Create a folder called `Autocad.Template.Basic`.
2. Add a `.template.config` folder to it.
3. Add a `.text` file to it, open it and add the json styled text below. Change it to your preferences (definitions [here](https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates)). Most of them do not need clarification, but we will explore the important once later on in this tutorial.
    ```json
    {
      "$schema": "http://json.schemastore.org/template",
      "author": "Luc van Dijk",
      "classifications": [ "Autocad", "Plugin" ],
      "identity": "AutocadTemplateBasic",
      "name": "Autocad Template",
      "shortName": "ac-temp",
      "sourceName": "Autocad.Template.Basic",
      "preferNameDirectory":true,
      "tags": {
        "language": "C#",
        "type": "project"
      }
    }
    ```
4. Save the file as 'template.json' and remove the earlier created `.txt` file.

We now have the previously shown structure and that means that we have created our first template. We can quickly test it by opening a terminal, go in the directory of `...\Autocad.Template.Basic`. Now run the command `dotnet new install .` and your template is ready for use. If you do a `dotnet new list` you will see your template there. You can also open a code editor, create a new project and then find your newest template there.

If you create an implementation of the template, the `.template.config` folder has disappeared. This is the default behaviour of a template.

We will be updating the template quite a bit in this tutorial and every time you want to see the changes, reinstall by running the following commands in the terminal:

- `dotnet new uninstall <path\to\your\template>`

- `dotnet new install .` (in your template folder)

### Adding file/folder to your project template
The next step is adding some structure and some files to a project. In my case I would like to have a `.csproj` that has the setup for my project already partially defined.

Let's do it:
1. To connect to the AutoCAD API, we need to install the right version of [AutoCAD ObjectARX](https://aps.autodesk.com/developer/overview/objectarx-autocad-sdk). The download link is hidden behind the button "View license agreement". Download it and place it in a convenient place on your computer. For this tutorial we are only interested in a couple of `.dll` files in the `inc` folder. You can skip this step or download another package if you are not here for the AutoCAD part.
2. Create a new `.txt` file in the `Autocad.Template.Basic` folder.
3. Open the file and add the following xml code:
    ```xml
    <Project Sdk="Microsoft.NET.Sdk">
    
        <PropertyGroup>
            <TargetFramework>net8.0</TargetFramework>
            <ImplicitUsings>enable</ImplicitUsings>
            <Nullable>enable</Nullable>
            <RootNamespace>Autocad.Template.Basic</RootNamespace>
        </PropertyGroup>
        
        <ItemGroup>
            <Reference Include="accoremgd">
                <HintPath>C:\Autocad 2026 ObjectARX\inc\AcCoreMgd.dll</HintPath>
                <Private>False</Private>
            </Reference>
            <Reference Include="Acdbmgd">
                <HintPath>C:\Autocad 2026 ObjectARX\inc\AcDbMgd.dll</HintPath>
                <Private>False</Private>
            </Reference>
            <Reference Include="acmgd">
                <HintPath>C:\Autocad 2026 ObjectARX\inc\AcMgd.dll</HintPath>
                <Private>False</Private>
            </Reference>
        </ItemGroup>
    
    </Project>
    ```
4. Change the `RootNamespace` to the value of `sourceName` in the `template.json` file. This is important because every instance of this value will be replaced to the _name of the implementation of the template_, which is really handy. We will look at this after we reinstall the template. 
5. Change the paths of the references to `accoremgd.dll`, `Acdbmgd.dll` and `acmgd.dll` to where they are located on your device.
6. Save the file as `Autocad.Template.Basic.csproj` and remove the `.txt` file.

Reinstall your template like earlier and then create a project that is called `TestSourceName` for example. When you inspect the project you will see the following changes:
- `Autocad.Template.Basic.csproj` -> `TestSourceName.csproj`
- `<RootNamespace>Autocad.Template.Basic</RootNamespace>` -> `<RootNamespace>TestSourceName</RootNamespace>`

Pretty useful, right? 

Let's start working in a code editor now. Pick one you like, and open the `Autocad.Template.Basic.csproj` in this editor.

Since the `.template.config` was not indexed with the project, it might not be visible in your code editor. We want to keep the indexing the way it is, but you can still edit the file by clicking on the `Show all files` in the solution tree in your IDE.

#### Adding a launch profile
If you are creating .NET projects, you want to explain the compiler what you mean with "debugging". For example, you probably want to start the Autocad application whenever we run a debug in this project. This is done in a launch profile, and you could add it to your project every time you create a plugin, but why not make this easy?

1. Add a `Properties` folder to the project.
2. Add a file called `launchSettings.json` to this folder. In this file you can add 1 or more profiles that will be available when debugging.
3. Add the following example profile to the new file, but change the `executablePath` to the path of your `acad.exe` location. Feel free to add other profiles whenever you like.
    ```json
    {
      "profiles": {
        "Autocad 2026": {
          "commandName": "Executable",
          "executablePath": "C:\\Program Files\\Autodesk\\AutoCAD 2026\\acad.exe",
          "commandLineArgs": "/nologo /b \"start.scr\""
        }
      }
    }
    ```
By reinstalling the project and opening a new implementation, you will see a new launch profile is available in your editor called `Autocad 2026`. You can now debug the project and see that Autocad is launching, but it will report that the "magical" `start.scr` that we mentioned in the `commandLineArgs` was not found. We will discuss it next.

#### Adding a start.scr

Whenever you want to load a plugin, you have to run a command in AutoCAD. This means that every time you want to try its latest feature of your plugin, you have to manually do this. This process can and should be automated by you. The command in AutoCAD is `NETLOAD "<YourPlugin.dll> "` and if we put this command in the `.scr` file and update it to the output folder, we can run this script via the `commandLineArgs` in the `launchsettings.json`. Follow along if this sounds like abracadabra to you.

1. Add a file called `start.scr` to the project. Open the file and add `NETLOAD "Autocad.Template.Basic.dll "`. **The space at the end is intentional** and recognize that we use the sourceName-replacing-trick again.
2. To copy this `start.scr` to the output folder for debugging, add an item group to the `Autocad.Template.Basic.dll`. It should now look like this:
```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <RootNamespace>Autocad.Template.Basic</RootNamespace>
    </PropertyGroup>
    
    <ItemGroup>
        <Reference Include="accoremgd">
            <HintPath>C:\Autocad 2026 ObjectARX\inc\AcCoreMgd.dll</HintPath>
            <Private>False</Private>
        </Reference>
        <Reference Include="Acdbmgd">
            <HintPath>C:\Autocad 2026 ObjectARX\inc\AcDbMgd.dll</HintPath>
            <Private>False</Private>
        </Reference>
        <Reference Include="acmgd">
            <HintPath>C:\Autocad 2026 ObjectARX\inc\AcMgd.dll</HintPath>
            <Private>False</Private>
        </Reference>
    </ItemGroup>

    <ItemGroup>
        <None Update="start.scr">
            <CopyToOutputDirectory>Always</CopyToOutputDirectory>
        </None>
    </ItemGroup>

</Project>
```
Again, reinstall and open a new test project with your template. Debug and check the command line in AutoCAD, because there should be a pop-up asking you if you want to load the plugin (accept or always accept it, it doesn't matter for now) and a message that says:

`NETLOAD Assembly file name: "<NameOfImplementation>.dll"`

If it does not work you should run `LEGACYCODESEARCH` in AutoCAD and make sure that is set to `ON`. You can now simply restart the debugging session and the message will be there.

Hint: You can also set `SECURELOAD` to `FALSE` in AutoCAD to not get the pop-up asking if you want to load your plugin. Only use these settings while debugging.

#### Adding the Active class & Commands class
There are lots of other things you can do when programming in AutoCAD, but there is a big chance that you are here to create new custom commands. For the creation of these commands, you are very likely to encounter a couple of objects that let you manipulate AutoCAD:

- The active document
- The active editor
- The active database

Active means that this is a part of the program that the user is interacting with. You could also be working with inactive documents for example. The starting point of you plugin is almost always in the active workspace, so we could at some infrastructure to interact with these objects. This idea comes from a [class of Ben Rand](https://www.autodesk.com/autodesk-university/class/Create-Your-First-AutoCAD-Plug-2020#video), which I would highly recommend to watch to get an idea of what an AutoCAD plugin could do. If it doesn't make sense jet, just follow along. Everything will become clear once you see a simple example.

Create a new file in the project called `Active.cs` and add the follow code:
```csharp
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace Autocad.Template.Basic;

public static class Active
{
   public static Document Document 
      => Application.DocumentManager.MdiActiveDocument;
   
   public static Editor Editor 
      => Document.Editor;
   
   public static Database Database 
      => Document.Database;
}
```
This makes accessing these objects a little bit simpler. For example, accessing the editor to write a message is normally done by:

`Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nHello, World!")`

And now becomes:

`Active.Editor.WriteMessage("\nHello, World!")`

Again, the namespace name in this file will be changed, like we have seen a couple of times by now.

Another useful file to add is a `Commands.cs` file containing a class that collects all the commands you will create. These commands have an attribute called CommandMethod that will do all the setup for you to run it from the command line in AutoCAD. There are a couple of parameters that this attribute can have, but we will not explore that here. This should be in the `Commands.cs` file:
```csharp
using Autodesk.AutoCAD.Runtime;

namespace Autocad.Template.Basic;

public class Commands
{
   [CommandMethod("Test")]
   public static void Test()
   {
      Active.Editor.WriteMessage("\nHello, World!");
   }
}
```
This very simple method will run when the user types `Test` in AutoCAD, resulting in a "Hello, World!" appearance in the editor.

### Packaging
Great! This is already a good template for personal use. To share this requires you to package your template. A package can contain a couple of templates, and it needs a special `.csproj` file that is styled in a way that .NET knows how to pack. It should be located at the same level as the folders of the templates. Here is an example of such a folder with 2 templates.
```
Autocad.Templates
├───Autocad.Templates.csproj
└───Templates
    ├───Template1
    │   ├───.template.config 
    │   │   └───template.json
    │   │
    │   ├───file1.cs
    │   ...
    │   
    └───Template2
        ├───.template.config 
        │   └───template.json
        │
        ├───file1.cs
        ...
```
For our project we only have 1 template, but we have to pack it like this anyway. The good thing about this is of course that you can add as many templates to this package as you like. So:

1. Create a new folder called "Autocad.Templates" or whatever you think is a good name for you package.
2. Add a `Autocad.Templates.csproj` file to this folder like you did in the beginning of this tutorial. It should contain the following code, but feel free to make it to your likings:
   ```xml
   <Project Sdk="Microsoft.Net.Sdk">
       
       <PropertyGroup>
           <PackageType>Template</PackageType>
           <PackageVersion>1.0</PackageVersion>
           <PackageId>Lucs.Autocad.Templates</PackageId>
           <Title>Autocad plugin template for C#</Title>
           <Authors>Luc van Dijk</Authors>
           <Description>A template for creating a new Autocad plugin using C#</Description>
           <PackageTags>templates;plugin;AutoCAD</PackageTags>
           <TargetFramework>net8.0</TargetFramework>
           
           <IncludeContentInPack>true</IncludeContentInPack>
           <IncludeBuildOutput>false</IncludeBuildOutput>
           <ContentTargetFolders>content</ContentTargetFolders>
       </PropertyGroup>
       
       <ItemGroup>
           <Content Include="Templates\**\*" Exclude="Templates\**\bin\**;Templates\**\obj**" />
           <Compile Remove="**\*" />
       </ItemGroup>
   </Project>
   ```
   Here is a quick overview of the most important features:
   - PackageType: is "Template", letting the compiler know what to expect.
   - PackageId: is the name that the user of you package calls to install all the templates in the package.
   - PackageTags: helps users on NuGet find you package.
   - The ItemGroup at the bottom removes all the `bin` and `obj` folders from the package. These get created when you work on your template in a code editor.
3. Create a folder called `Templates` in the `Autocad.Templates` folder. This folder contains all the templates that you have in the package.
4. Copy and paste the `Autocad.Template.Basic` project to this `Templates` folder.

This should now be the structure we have created:
```
Autocad.Templates
├───Autocad.Templates.csproj
└───Templates
    └───Autocad.Template.Basic
        ├───Autocad.Template.Basic.csproj
        │
        ├───.template.config 
        │   └───template.json
        │
        ├───Properties
        │   └───launchSettings.json
        │
        ├───Active.cs
        ├───Commands.cs
        └───start.scr

```
We are ready for packaging:
1. Open any terminal.
2. Go in the `Autocad.Templates` folder
3. Run `dotnet pack`

You can find your new, sharable, NuGet package the `bin/Release/net8.0` folder of this project. To install the package, run this command in the `Autocad.Templates` folder:

`dotnet new install .\bin\Release\net8.0\Autocad.Templates.1.0.0.nupkg`

For testing purposes you can always uninstall the package and then package and reinstall it by doing the following steps:
1. `dotnet new uninstall <package name>`
2. `dotnet pack` in the folder of you package.
3. `dotnet new install <path\to\.nupkg\file>`

## Advanced Templates
Since we now have a nice template, we can start working on our new Autocad Plugins, but there is a lot more that you can do with templates. To add more flexibility to your template, you are preventing creating lots of branches of really similar templates. You can easily go to far with adding flexibility and arrive at a point where setting up the template is more complicated that starting from scratch. Keep your templates general and to the point, so you can use it for any case. Let's look at an example.

### Adding symbols
If you are working with a specific use case it would be nice to have an option where a drawing (`.dwg` file) automatically starts when you debug your plugin. For example, you are working on a plugin that collects all the text objects in a drawing, and you want to save their text values to a database. It would be nice to open AutoCAD with a drawing containing a couple of text objects, so you can start testing straight away. 

Open AutoCAD and create a new file. Save it as `debug.dwg` to the project folder.

We need to create a parameter that the user of the template can enable and disable. The `template.json` has options to do this. We first add a new item that is called `symbols`. This contains all the custom symbols that we want to add to our project. We will add one that is called "includeStartUpDrawing" which is a boolean value that is set to "false" by default. Here is the changed `template.json`

```json
{
   "$schema": "http://json.schemastore.org/template",
   "author": "Luc van Dijk",
   "classifications": [ "Autocad", "Plugin" ],
   "identity": "AutocadTemplateBasic",
   "name": "Autocad Template",
   "shortName": "ac-temp",
   "sourceName": "Autocad.Template.Basic",
   "preferNameDirectory":true,
   "tags": {
     "language": "C#",
     "type": "project"
   },
   "symbols": {
      "includeStartUpDrawing": {
         "type": "parameter",
         "dataType": "bool",
         "defaultValue": "false",
         "displayName": "Include start up drawing",
         "description": "Includes a .dwg file that always starts when debugging."
      }
   },
   "sources": [
      {
         "modifiers": [
            {
               "condition": "(!includeStartUpDrawing)",
               "exclude": ["debug.dwg"]
            }
         ]
      }
   ]
}
```
As you can see, there is a new symbol, but also a `sources` section. This is a list where we can add conditions to specific sources (folders) in our project. By default, we are working in the project folder, but we can also target specific folders using a `source`. We now just want to remove a file conditionally from the main source, so we add a `modifiers` list where we add the `condition` and what we want to `exclude` if this condition is true. You can discover more about `sources` and in the [reference guide](https://github.com/dotnet/templating/wiki/Reference-for-template.json).

When you now, after reinstalling, open a new solution in you IDE, you will see an option in the "Advanced Settings" section that has our new checkbox. 

### Adding conditional pieces of code

Having a just a checkbox and then removing a file is nice, but it is not enough: we still need to add some code to make it work. Let's dive in:

- Updating the launch settings
   
   In order to start this new drawing file, we want to add it to the `commandLineArgs` in the `launchSettings`, but only when `includeStartUpDrawing` is set to true. We have a weird syntax that is specific for templates and on top of that depend on the file that you are working on. [here](https://github.com/dotnet/templating/wiki/Conditional-processing-and-comment-syntax) is a guide for conditionally formatting your templates with various file types. We just want to have an if-else statement that starts AutoCAD either with or without the `debug.dwg`:
   ```json
   {
     "profiles": {
       "Autocad 2026": {
         "commandName": "Executable",
         "executablePath": "C:\\Program Files\\Autodesk\\AutoCAD 2026\\acad.exe",
         //#if (includeStartUpDrawing)
         "commandLineArgs": "\"debug.dwg\" /nologo /b \"start.scr\""
         ////#else
         "commandLineArgs": "/nologo /b \"start.scr\""
         //#endif
       }
     }
   }
   ``` 
  Don't worry about the big red and yellow lines that your code editor places below the newly added lines. The problem is that `.json` files do not have commenting available, though when you implement this template, the "comments" and duplication problems will be gone.


-  Updating the `.csproj` file

   The same goes for the `Autocad.Template.Basic.csproj`: we conditionally add a section that copies the `debug.dwg` file over to the output folder. Again, use [this](https://github.com/dotnet/templating/wiki/Conditional-processing-and-comment-syntax) as a reference for creating these if-statements, but now look for the `xml` file conditions.

   ```xml
   <Project Sdk="Microsoft.NET.Sdk">
   
       <PropertyGroup>
           <TargetFramework>net8.0</TargetFramework>
           <ImplicitUsings>enable</ImplicitUsings>
           <Nullable>enable</Nullable>
           <RootNamespace>Autocad.Template.Basic</RootNamespace>
       </PropertyGroup>
   
       <ItemGroup>
           <Reference Include="accoremgd">
               <HintPath>C:\Autocad 2026 ObjectARX\inc\AcCoreMgd.dll</HintPath>
               <Private>False</Private>
           </Reference>
           <Reference Include="Acdbmgd">
               <HintPath>C:\Autocad 2026 ObjectARX\inc\AcDbMgd.dll</HintPath>
               <Private>False</Private>
           </Reference>
           <Reference Include="acmgd">
               <HintPath>C:\Autocad 2026 ObjectARX\inc\AcMgd.dll</HintPath>
               <Private>False</Private>
           </Reference>
       </ItemGroup>
   
       <ItemGroup>
           <None Update="start.scr">
               <CopyToOutputDirectory>Always</CopyToOutputDirectory>
           </None>
           <!--#if (includeStartUpDrawing) -->
           <None Update="debug.dwg">
               <CopyToOutputDirectory>Always</CopyToOutputDirectory>
           </None>
           <!--#endif -->
       </ItemGroup>
   
   </Project>
   ```

Save everything, package and reinstall the template.

If you followed along, your template now has the following features:
- Starting AutoCAD with an optional test file when debugging.
- Loading the newly compiled `.dll` in AutoCAD.
- The basic infrastructure to interact with AutoCAD.
- A placeholder method for quickly testing if everything is ready for your coding.

## Reflecting on your templates
When adding new feature to your template, you should be careful to not complicate the template. It is a balancing act between having an indepth and complex template versus a general-purpose and easy template. Keep it to the point as much as possible. What do you or the users really need to get out of it? Does this make it easier than starting of from scratch? Is this new feature an often reoccurring thing? 

## Conclusion
Custom templates for .NET projects like AutoCAD plugins can help starting your project faster and more consistent. The template created in this tutorial shows how you can easily add files, get inputs from the user of the template and turn files and pieces of code on and off, but there is a lot more to explore. Explore by creating your own templates and prevent writing reoccurring code








