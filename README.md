# Bio Workflow Engine (BioWF)
.NET Bioinformatics Workflow designer and execution engine.

## Welcome to the BioWF project

The BioWF project is a recreation of the Trident Scientific Workbench from MSR but using Windows Workflow 4.5 and .NET Bio 1.1. It allows you to create programs visually by connecting various components together in the designer and then persist them to an XML file which can either be run in the GUI designer, or from a command line tool.

## Dependencies
This project uses the [JulMar MVVMHelpers library](https://github.com/markjulmar/mvvmhelpers) and [.NET Bio](https://github.com/dotnetbio/bio), both available in NuGet.

## Examples
Check out <https://markjulmar.github.io/bio/2013/07/26/introducing-biowf-workflow-bioinformatics-designer.html> for a very simple example of how to use it.

## Compiling the code

I have updated the code to Visual Studio 2017 and the latest .NET Bio pre-release which utilizes .NET Standard. It should build on any version of VS2017 for Windows. Note that since this uses .NET Desktop, this app will only compile and run on Windows.
