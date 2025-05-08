# Create your personal template for .NET
Starting a new project can take up some unnecessary time. You pick your favorite template from the standard library and start coding, but you realize that you could be copying code from a previous project. A custom template could be a good idea. 

I noticed while creating AutoCAD plugins that the starting point of each project is always the same, and I was sometimes copying code from one project to the other. It took me some time to learn about project templates, but it definitely helps speeding up the start of my newest plugin. The other advantage is that you always have the same, if done correctly, clean base upon which you just extend to create your program. So a template makes the starts of your projects faster and more consistent to each other.

This tutorial will focus on making an AutoCAD plugin template package, but whether you are here for the AutoCAD part or just want to understand the templates for .NET, following this tutorial will enable you to create complex templates using basic building blocks. 

### The backbone of a template
There is one key element that all of templates need: The `template.json` file. It contains all the configuration data for the implementation of your template. Once this file is added to a `.template.config` folder, .NET will be able to understand that the files and folders that are in the same folder as the .template.config folder belong to a template. Let's create an example template without using any code editor.

[comment]: <> (tree signs: │ ├─── └───)
```
EmptyTemplate
└───.template.config 
    │   template.json
```

1. Create a folder called `EmptyTemplate`.
2. Add a `.template.config` folder to it.
3. Add a `.text` file to it, open it and add the following json styled text:
```
{
  "$schema": "http://json.schemastore.org/template",
  "author": "Luc van Dijk",
  "classifications": [ "Empty" ],
  "identity": "EmptyTemplate",
  "name": "Empty Template",
  "shortName": "empty",
  "sourceName": "Empty",
  "preferNameDirectory":true,
  "tags": {
    "language": "C#",
    "type": "project"
  }
}
```
You can find [here](https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates) what all the items mean. We will explore the important once later in this tutorial.
 
### Adding a standard file/folder to you project
When debugging an AutoCAD plugin, you want to start the application to run your newly edited command. To not have to manually do this, we first want to create a properties folder with a launchSettings.json. These launch settings will create a debug profile in the code editor, which is really useful for any application. Lets start with the creating these files.
### Create a templates
In the image bellow you can see the structure that we will be using for this tutorial. There is a folder called AutoCAD.Basic.Template. For now, this folder just contains a start.scr file and a properties folder (we will get back to these) and in addition to that it has the .template.config folder with the template.json file inside. The way I like to do this:
1.	Open the a file explorer
2.	Create a folder called templates wherever you like.
3.	In the Templates folder:
a.	Create a folder called Basic.
b.	Add a text file to the folder. Open the text file and directly save it as a Basic.csproj
4.	Open Basic.csproj in the code editor of you choice. I will use Rider for this tutorial. It will give an error since it is an empty project file. Right-click on it and click on Edit > Edit ‘Basic.csproj’
This is actually all you need to create you first template. In order to implement it we must install it first. To do this, open a terminal and go into the folder were the template is located. If you are coding along in Rider, you can just use the build in terminal. Once you located the 
Download these beforehand
-	Autodesk AutoCAD 2026.
-	The objectARX for AutoCAD 2026. It is not that clear where to find this:
o	Go to this website.
https://aps.autodesk.com/developer/overview/objectarx-autocad-sdk
o	Scroll down to “View license agreement”
o	Agree to the terms and let them agree that you are not a robot
o	Download the right version for your device
o	Run the installer and place the files on a convenient place
-	A code editor of your choice. I use Rider by Jetbrains demonstration purposes, since I think that there already are quite some guides for creating templates in Visual Studio. The same principles apply for both.
Let’s start with creating a template in Rider

No matter which IDE you pick to you have to create a coupling between AutoCAD and your editor and there are some steps that you could optionally take to create your plugin. Most of the business is happening in the project file (.csproj), so we can create a template for it. This way, we can pick the template as a starting point for all of our upcoming projects. Here are the steps:
1.	Create a class
2.	Target the right .NET edition, which is .NET 8.0 in AutoCAD 2026, but could be something else by the time you are reading this. Check it out here.
https://help.autodesk.com/view/OARX/2025/ENU/?guid=GUID-450FD531-B6F6-4BAE-9A8C-8230AAC48CB4
3.	Reference all the packages that are necessary for the code to understand what objects and functions are available in AutoCAD. These packages are downloaded along with AutoCAD.
4.	Automatically start the AutoCAD application when you want debug.
GileCAD already created this template for you. Check out his repo and adjust it for your needs. I would highly recommend reading the README.md if you are new to these kind of setups, since it explains it really clearly. It also states which file paths must be changed to find the right files on your device.

What is the “best” IDE to use for creating Autocad Plugins?
Of course, it totally depends on you. But there are some aspects that make one IDE more advantageous over the others. Lets quickly look at some of the code editors that you might consider.
Still in? Lets see what some code editors have to offer to you. 
Rider by Jetbrains
I first used Rider for all my C# coding, since I used it in university and also because I’ve used Pycharm for some coding in Python. Rider is nice and clean, but 

