# FindBiggestFile

Is a program written in [FSharp](https://dotnet.microsoft.com/languages/fsharp) using [.Net 5](https://dotnet.microsoft.com/download/dotnet/5.0) for finding the biggest files from a config file you supply to it using cli arguments and a json config file.

## Build

Build it using ```dotnet build -c Release```.

## Create Example Config

Running it with ```./FindBiggestFile.exe --init``` generates an example config file for you to fill out. You can also find the example config inside this [repository](/FindBiggestFile/Example_Config.json).

## Supplying a Config

Running it with ```.FindBiggestFile.exe --configfilepath Example_Config.json``` executes the search for your files.

## Finding the biggest file of a certain type in a certain folder

Using the case "BiggestFileInFolder" you can search for the biggest file with a certain file type in a given folder.
