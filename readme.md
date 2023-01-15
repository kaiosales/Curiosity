## Introduction

This is a simple .NET Core v7 application, showing a
rover robot simulation  capable of navigating a provided grid terrain 
using a provided set of commands

# Dependencies

- [dotnet Core v7](https://dotnet.microsoft.com/en-us/download)
- [Spectre.Console](https://github.com/spectreconsole/spectre.console)
- [xUnit](https://github.com/xunit/xunit)
- [Moq](https://github.com/moq/moq4)

## What's contained in this solution

This solution is divided into 2 main projects (and a test project).

The first project `Curiosity.Domain` contains all business logic and data
processing required for the solution to work including the main classes 
`Transmitter` and `Rover`.
The second project `Curiosity.UI.Console` is just the UI side leveraging 
`Spectre.Console` library for a more immersive "space experience".

## Instructions
First, install dotnet core and clone this project

### Build
Navigate to the root level and build using the dotnet cli, this should 
install all `Nuget` dependencies:

```
dotnet build 
```

### Test
This project contains unit tests developed using `xUnit` and can be ran
using the standard dotnet cli:

```
dotnet test 
```

### Run
Finally to run this project, you'll need to target the `Curiosity.UI.Console`
project when using dotnet cli:

```
dotnet run --project .\Curiosity.UI.Console\
```

![alt text](assets/demo.gif)