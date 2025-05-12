# Create your personal template for .NET
Starting a new project can take up some unnecessary time. You pick your favorite template from the standard library and start coding, but you realize that you could be copying code from a previous project. A custom template could be a good idea. 

I noticed while creating AutoCAD plugins that the starting point of each project is always the same, and I was sometimes copying code from one project to the other. It took me some time to learn about project templates, but it definitely helps speeding up the start of my newest plugin. The other advantage is that you always have the same, if done correctly, clean base upon which you just extend to create your program. So a template makes the starts of your projects faster and more consistent to each other.

This tutorial will focus on making an AutoCAD plugin template package, but whether you are here for the AutoCAD part or just want to understand the templates for .NET, following this tutorial will enable you to create complex templates using basic building blocks. 

### The backbone of a template
There is one key element that all templates need: The `template.json` file. It contains all the configuration data for the implementation of your template. Once this file is added to a `.template.config` folder, .NET will be able to understand that the files and folders that are in the same folder as the .template.config folder belong to a template. Let's create an example template without using any code editor.

[comment]: <> (tree signs: │ ├─── └───)
```
AutocadTemplate
└───.template.config 
    │   template.json
```

1. Create a folder called `AutocadTemplate`.
2. Add a `.template.config` folder to it.
3. Add a `.text` file to it, open it and add the json styled text below. You can find [here](https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates) what all the items mean. Most of them do not need clarification, but we will explore the important once later on in this tutorial.
    ```json
    {
      "$schema": "http://json.schemastore.org/template",
      "author": "Luc van Dijk",
      "classifications": [ "Autocad", "Plugin" ],
      "identity": "AutocadTemplate",
      "name": "Autocad Template",
      "shortName": "ac-temp",
      "sourceName": "AutocadTemplate",
      "preferNameDirectory":true,
      "tags": {
        "language": "C#",
        "type": "project"
      }
    }
    ```
4. Save the file as 'template.json' and remove the earlier created `.txt` file.

We now have the previously shown structure and that means that we have created our first template. We can quickly test it by opening a terminal, go in the directory of `...\AutocadTemplate`. Now run the command `dotnet new install .` and your template is ready for use. If you do a `dotnet new list` you will see your template there. You can also open a code editor, create a new project and then find your newest template there.

If you create an implementation of the template, the `.template.config` folder has disappeared. This is the default behaviour of a template.

We will be updating the template quite a bit in this tutorial and every time you want to see the changes, reinstall:

- `dotnet new uninstall <path\to\your\template>`

- `dotnet new install .` (in your template folder)

### Adding file/folder to your project template
The next step is adding some structure and some files to a project. In my case I would like to have a `.csproj` that has the setup for my project already partially defined.

Let's do it:
1. Before we start, we need to install the [AutoCAD ObjectARX](https://aps.autodesk.com/developer/overview/objectarx-autocad-sdk). The download link is hidden behind the button "View license agreement". Download it and place it in a convenient place on your computer. For this tutorial we are only interested in a couple of `.dll` files in the `inc` folder. You can skip this step or download another package if you are not here for the AutoCAD part.
2. Create a new `.txt` file in the `AutocadTemplate` folder.
3. Open the file and add the following code.
    ```xml
    <Project Sdk="Microsoft.NET.Sdk">
    
        <PropertyGroup>
            <TargetFramework>net8.0</TargetFramework>
            <ImplicitUsings>enable</ImplicitUsings>
            <Nullable>enable</Nullable>
            <RootNamespace>AutocadTemplate</RootNamespace>
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
5. Change the paths of the references to `accoremgd.dll`, `Acdbmgd.dll` and `acmgd.dll` to the location of the .
6. Save the file as `AutocadTemplate.csproj` and remove the `.txt` file.

Reinstall your template like earlier and then create a project that is called `TestSourceName` for example. When you inspect the project you will see the following changes:
- `AutocadTemplate.csproj` -> `TestSourceName.csproj`
- `<RootNamespace>AutocadTemplate</RootNamespace>` -> `<RootNamespace>TestSourceName</RootNamespace>`

Pretty useful, right? 

Let's start working in a code editor now. Pick one you like, and open the `AutocadTemplate.csproj` in this editor.

Since the `.template.config` was not indexed with the project, it might not be visible in your code editor. We want to keep the indexing the way it is, but you can still edit the file by clicking on the `Show all files` in your editor.

### Adding a launch profile to start Autocad automatically
We probably want to start the Autocad application whenever we run a debug. You could add a launch profile to your editor every time you create a plugin, but why not make this easy?

1. Add a `Properties` folder to the project.
2. Add a file called `launchSettings.json` to this folder.
3. Add the following to the new file, but change the `executablePath` to the path to your `acad.exe` location.
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

### Loading the plugin automatically

We want to run a command in AutoCAD that loads the `.dll` file. This process can and should be automated by you, because it needs to be done every time you debug. The command is `NETLOAD "<YourPlugin.dll> "` and if we put this command in the `.scr` file, we can update it to the output folder. This way it is easy to tell AutoCAD to run this when it starts up from our project.

1. Add a file called `start.scr` to the project. Open the file and add `NETLOAD "AutocadTemplate.dll "`. **The space at the end is intentional** and recognize that this is the sourceName-replacing-trick again.
2. To add this `start.scr` to the output folder for debugging, add a little bit to the `AutocadTemplate.dll`. It should now look like this:
```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <RootNamespace>AutocadTemplate</RootNamespace>
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
Again, reinstall and check out if it works. There should be a message that says:

`NETLOAD Assembly file name: "<NameOfImplementation>.dll"`

If it still does not work you should run `LEGACYCODESEARCH` in AutoCAD and make sure that is set to `ON`. 

[HINT] You can also set `SECURELOAD` to `FALSE` in AutoCAD to not get the pop-up asking if you want to load your plugin. Only use these settings while debugging.

### The Active Class & Commands Class
When you create a new plugin, you probably want to add a new command to AutoCAD. There are lots of other things you can do, but the reoccurring thing is creating these new commands. For the creation of these commands, you are very likely to encounter a couple of objects that let you manipulate the program:

- The active document
- The active editor
- The active database

Active means that this is a part of the program that the user is interacting with. You could also be working with inactive documents for example. The starting point of you plugin is almost always in the active workspace, so we could at some infrastructure to interact with these objects. This idea comes from a [class of Ben Rand](https://www.autodesk.com/autodesk-university/class/Create-Your-First-AutoCAD-Plug-2020#video), which I would highly recommend to watch to get an idea of what an AutoCAD plugin could do. If it doesn't make sense jet, just follow along. Everything will become clear once you see a simple example.

Create a new file in the project called `Active.cs` and add the follow code:
```csharp
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace AutocadTemplate;

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

`Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nHello")`

And now becomes:

`Active.Editor.WriteMessage("\nHello")`

Again, the namespace name will be changed, like we have seen a couple of times by now.

Another useful file to add is a `Commands.cs` file containing a class that collects all the commands you will create. These commands have an attribute called CommandMethod that will do all the setup for you to run it from the command line in AutoCAD. There are lot more options that this attribute can have to create this setup, but we will not explore that here. This should be in the `Commands.cs` file:
```csharp
using Autodesk.AutoCAD.Runtime;

namespace AutocadTemplate;

public class Commands
{
   [CommandMethod("Test")]
   public static void Test()
   {
      Active.Editor.WriteMessage("\nHello World!");
   }
}
```
This very simple method will run when the user types `Test` and it will then write "Hello World!" in the editor.

Save everything and reinstall the template. Before we package, lets checkout what we actually have right now.

When we use the template for a new project and start debugging the project 
- automatically starts AutoCAD.
- loads the newly compiled `.dll` in AutoCAD.
- has some basic infrastructure to interact with AutoCAD.
- has a placeholder method for quickly testing if everything is ready for your coding.

Great! This is already really useful for yourself. To share this requires you to package your templates. We need a special `.csproj` file is styled in a way that .NET knows how to pack and should be located in the folder with the templates. Here is an example of such a folder with 2 templates.
```
AutocadTemplates
├───AutocadTemplates.csproj
└───Templates
    ├───Template1
    │   ├───.template.config 
    │   │   │   template.json
    │   │
    │   ├───file1.cs
    │   ...
    │   
    └───Template2
        ├───.template.config 
        │   │   template.json
        │
        ├───file1.cs
        ...
```
For our project we only have 1 template, but we have to pack it like in this form anyway. The good thing about this is of course that you can add as many templates to this package as you like. So:

1. Create a new folder called "AutocadTemplates" or whatever you think is a good name for you package.
2. Add a `AutocadTemplates.csproj` file to this folder.
3. Add the follow code to it:
   ```xml
   <Project Sdk="Microsoft.Net.Sdk">
       
       <PropertyGroup>
           <PackageType>Template</PackageType>
           <PackageVersion>1.0</PackageVersion>
           <PackageId>LucsAutocadTemplate</PackageId>
           <Title>Autocad plugin template for C#</Title>
           <Authors>Luc van Dijk</Authors>
           <Description>A template for creating a new Autocad plugin using C#</Description>
           <PackageTags>templates;dotnet;AutoCad</PackageTags>
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
   - PackageType: is "Template", letting the packager know what to expect.
   - PackageId: is the name that the user of you package calls to install all the templates in the package.
   - PackageTags: helps users on NuGet find you package.
   - The ItemGroup at the bottom filters out all the `bin` and `obj` folders that get created when you work on your template in a code editor.
4. Create a folder called `Templates` in the same folder as the `AutocadTemplates.csproj`. This folder will contain all the templates that you want to add to the package.
5. Copy and past the template project to this `Templates` folder.

We are ready for packing:
1. Open a terminal.
2. Go to the `AutocadTemplates` folder
3. Run `dotnet pack`

You can find your new NuGet package the `bin/Release/net8.0` folder of this project. To install the package, you have to install 






