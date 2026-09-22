# Asset Tracker

A C# / .NET 10 console application for managing company assets such as
laptops, phones, monitors and other equipment.

The project demonstrates object-oriented programming, file handling,
searching, sorting, validation, CSV export and clean code principles.

## Project Information

**Project:** Asset Tracker\
**Language:** C#\
**Framework:** .NET 10\
**Application type:** Console Application\
**Repository:** https://github.com/luisianodc/AssetTracker.git

## Main Features

The application can:

-   Add assets
-   Display assets
-   Search for assets
-   Sort assets
-   Edit asset information
-   Manage offices and currencies
-   Save and load asset data using files
-   Export asset information to CSV
-   Validate user input
-   Handle duplicate asset IDs
-   Work with multiple offices

## Technologies

-   C#
-   .NET 10
-   Object-Oriented Programming
-   File handling
-   CSV export
-   Git and GitHub
-   Visual Studio
-   JetBrains Rider
-   Command Prompt / Terminal

## Requirements

Install the following before running the project:

-   .NET 10 SDK
-   Visual Studio 2022/2026 with .NET development support, or
-   JetBrains Rider with .NET support

You can check your installed .NET version with:

``` text
dotnet --version
```

The result should show a .NET 10 SDK version.

## Running the Project with Visual Studio

1.  Clone or download the project from GitHub.
2.  Open the solution file:

``` text
AssetTracker.sln
```

3.  Open the solution in Visual Studio.
4.  Make sure the Asset Tracker console project is selected as the
    startup project.
5.  Build the solution:

``` text
Build > Build Solution
```

6.  Run the application by pressing:

``` text
F5
```

or:

``` text
Ctrl + F5
```

The console application will start.

## Running the Project with JetBrains Rider

1.  Clone or download the project from GitHub.
2.  Open JetBrains Rider.
3.  Select **Open**.
4.  Open:

``` text
AssetTracker.sln
```

5.  Allow Rider to restore the NuGet packages if required.
6.  Select the Asset Tracker project as the startup project.
7.  Run the application using the green **Run** button.

You can also use:

``` text
Shift + F10
```

to run the application.

## Running from Command Terminal

Open Command Prompt, PowerShell or another terminal.

Navigate to the project directory:

``` text
cd path\to\AssetTracker
```

Restore the project:

``` text
dotnet restore
```

Build the project:

``` text
dotnet build
```

Run the application:

``` text
dotnet run
```

## Useful .NET Commands

### Restore

``` text
dotnet restore
```

Restores the project's dependencies.

### Build

``` text
dotnet build
```

Compiles the application.

### Run

``` text
dotnet run
```

Builds and starts the application.

### Clean

``` text
dotnet clean
```

Removes previous build output.

### Build in Release mode

``` text
dotnet build --configuration Release
```

## Application Workflow

The application is designed around a simple console menu.

A typical workflow is:

1.  Start the application.
2.  Select an option from the main menu.
3.  Enter the requested information.
4.  Validate the input.
5.  Add, edit, search or sort assets.
6.  Save or export the information when required.
7.  Return to the main menu or exit the application.

## Asset Management

Each asset contains information such as:

-   Asset ID
-   Asset type
-   Brand
-   Model
-   Purchase date
-   Office
-   Other relevant asset information

Asset IDs are used to identify individual assets.

The application checks for duplicate IDs before adding an asset.

## Searching

The application supports searching for assets using text.

Search can be performed against relevant asset information such as:

-   Brand
-   Model

The search is case-insensitive.

For example:

``` text
Search: Lenovo
```

can find assets with a brand or model containing `Lenovo`.

## Sorting

The application contains sorting functionality for different
requirements.

Examples include:

### Sort by Asset Type and Purchase Date

Assets are first sorted by asset type and then by purchase date.

### Sort by Office and Purchase Date

Assets are first sorted by office and then by purchase date.

The sorting is performed without changing the original asset collection.

## Offices and Currencies

Assets can be associated with an office.

The project also demonstrates the use of currencies for different
offices.

Example offices include:

-   Sweden - SEK
-   USA - USD
-   Turkey - TRY
-   Germany - EUR

## File Handling

The application uses file handling to save and load asset information.

The general workflow is:

``` text
Application
     |
     v
Asset Service
     |
     v
Data Store
     |
     v
File
```

This separates the application logic from the code responsible for
storing data.

When the application loads data, it also checks for duplicate asset IDs
so that the same asset is not added more than once.

## CSV Export

Asset information can be exported to a CSV file.

CSV files are useful because they can be opened in applications such as:

-   Microsoft Excel
-   LibreOffice Calc
-   Google Sheets
-   Other data-processing applications

## Project Structure

The project follows a structured approach separating responsibilities
between classes.

A typical structure is:

``` text
AssetTracker/
│
├── AssetTracker.sln
│
├── AssetTracker/
│   ├── Program.cs
│   │
│   ├── Models/
│   │   ├── Asset.cs
│   │   ├── Office.cs
│   │   └── Currency.cs
│   │
│   ├── Services/
│   │   └── AssetService.cs
│   │
│   ├── Data/
│   │   └── DataStore.cs
│   │
│   └── ...
│
├── README.md
└── ...
```

The exact folder and class structure may change as the project develops.

## Object-Oriented Programming

The project uses several important object-oriented programming
principles.

### Classes

Different classes represent different concepts in the application.

For example:

``` text
Asset
Office
Currency
AssetService
DataStore
```

### Encapsulation

Data and operations are kept inside appropriate classes.

### Separation of Responsibilities

Different parts of the application have different responsibilities.

For example:

``` text
Program
   |
   v
User interaction

AssetService
   |
   v
Business logic

DataStore
   |
   v
File handling
```

This makes the application easier to understand and maintain.

## Clean Code

The project follows several clean code principles:

-   Small methods
-   Clear method names
-   Meaningful variable names
-   One responsibility per method where possible
-   Separation of business logic and file handling
-   Input validation
-   Comments where they add useful information
-   Avoiding unnecessary duplicated code

The project also uses traditional C# syntax in several places to make
the code easier to understand while learning C#.

## Problem Solving

During development, problems were handled by breaking them into smaller
parts.

A typical approach was:

``` text
Identify the problem
       |
       v
Understand the requirement
       |
       v
Break the problem into smaller tasks
       |
       v
Implement one part
       |
       v
Test
       |
       v
Fix errors
       |
       v
Refactor
```

This approach helps prevent large problems from becoming difficult to
manage.

## AI-Assisted Development

AI tools were used as a learning and development aid.

Examples include:

-   Explaining C# concepts
-   Reviewing code
-   Finding possible errors
-   Suggesting alternative implementations
-   Explaining file handling
-   Improving code structure
-   Helping with documentation
-   Generating examples to study

AI-generated code should always be understood, tested and adapted before
being included in a project.

The goal is to use AI as a development assistant rather than simply
copying code without understanding it.

## Git and GitHub

The project is version controlled using Git.

Common commands:

``` text
git status
```

Check the current repository status.

``` text
git add .
```

Stage changes.

``` text
git commit -m "Describe the changes"
```

Create a commit.

``` text
git push
```

Push commits to GitHub.

To push the first time when the remote branch has not yet been
configured:

``` text
git push -u origin main
```

## Typical Git Workflow

A simple workflow is:

``` text
Make changes
     |
     v
git status
     |
     v
git add .
     |
     v
git commit -m "Describe changes"
     |
     v
git push
```

It is useful to commit regularly with meaningful commit messages.

## Tips for Development

### 1. Build frequently

Run:

``` text
dotnet build
```

regularly while developing.

This makes it easier to identify which change introduced an error.

### 2. Test small changes

Avoid making many unrelated changes before testing.

### 3. Use meaningful names

For example:

``` text
SearchAssets
AddAsset
SortByOfficeAndPurchaseDate
```

are easier to understand than generic names such as:

``` text
DoStuff
Process
Method1
```

### 4. Keep methods focused

A method should preferably have one clear responsibility.

### 5. Use Git regularly

Commit working versions so that previous versions can be recovered if
necessary.

## Future Improvements

Possible future improvements include:

-   Database support
-   Entity Framework Core
-   User authentication
-   Role-based access
-   A graphical user interface
-   Web API
-   ASP.NET Core version
-   Unit tests
-   Automated integration tests
-   More advanced reporting
-   Improved CSV import/export
-   Cloud storage
-   Logging
-   Dependency Injection
-   Configuration files
-   Automated CI/CD with GitHub Actions

## Useful Resources

### C# Documentation

https://learn.microsoft.com/dotnet/csharp/

### .NET Documentation

https://learn.microsoft.com/dotnet/

### .NET CLI

https://learn.microsoft.com/dotnet/core/tools/

### Git Documentation

https://git-scm.com/doc

### GitHub Documentation

https://docs.github.com/

## Repository

GitHub repository:

https://github.com/luisianodc/AssetTracker.git

## Author

**Ivo**

Software Engineer / C# and .NET Developer

The project was developed as a practical learning project to demonstrate
C#, .NET 10, object-oriented programming, file handling, clean code,
Git/GitHub and software development practices.

## License

This project is intended as an educational and demonstration project.
