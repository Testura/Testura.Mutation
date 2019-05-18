![Unima Logo](https://i.imgur.com/ELDUHai.png)

Are you looking for ways to improve your unit tests quality and test coverage? Then Unima and mutation testing may be something for you.

Unima is a mutation testing tool for C# that verify the quality of your unit tests by injecting different mutations in your code and then checking how your unit tests react. If your unit tests: 

- Fails it mean that you found the mutation and you have good coverage
- Pass it means that the mutation survived and you miss coverage on the specific functionality.

![Example of mutation](https://i.imgur.com/ZFPbEyI.png)
*Example of a mutation* 

## Why should I use Unima?
 
- Quick and easy to set up
- Very flexible and it's easy to set up which files and lines you want to mutate.
- Can take your test coverage from good to great.
- Two different versions - Console and a WPF application. 

# Install

Simply go to releases and download the latest version (or build from source).

## Usage

### Console application 

![Console application](https://i.imgur.com/0xVUmXi.png)

First you have to create a json run config, for example: 


```c#
{
    "SolutionPath":  "path\\to\\your\\solution.sln",
    "TestProjects":  [
                         "*.Tests"
                     ],
    "BuildConfiguration":  "Debug",
    "TestRunner":  "dotnet",
    "NumberOfTestRunInstances":  5,
}
```

Then you run the exe from for example cmd by writing: 

```c#
path\to\Unima.Console.exe local --configPath "path/To/Json/Config" --outputPath "path/to/output/directory"
```

It will then:

1. Run a baseline (simply execute all your unit tests to make sure that everything is fine)
2. Create mutations 
3. Go through each mutation and run all unit tests (we run all unit test each mutation).

After execution you will get a couple of different result files (we will later add so you can filter which result you want):

- A trx file 
- A markdown file 
- A short summary in txt
- A html file 
- A json file

They contain your mutation score (killed mutations divided by total mutations) as well as details about all survived mutations.

#### Possible config values 

##### SolutionPath

Path to your C# solution that we should mutate/run tests from. 

#### TestProjects

A list of all test projects that we should run (we won't mutate test projects). For example:

```c#
    "TestProjects":  [
						 "*\src\sometestproject.csproj"
                         "*.Tests"
                     ],
```

#### BuildConfiguration

If we should mutate "Debug" or "Release" (it's important to build your project before trying to run the mutation tool!)

#### TestRunner

Currently we have three different flags for the test runner: 

- dotnet: Supports all test frameworks and .NET Core 
- nunit: Supports nunit non-core 
- xunit: Support xunit non-core 

In most cases it's best to just run "dotnet".

#### NumberOfTestRunInstances

We run all unit tests for all mutations and this property tells us how many sessions we should run in parallel.

#### IgnoredProjects

List of projects that we shouldn't mutate or run tests from. For example: 
```c#
    "IgnoredProjects":  [
                            "Name.Of.The.Project"
                        ],
```
#### Filter

Filter is a way for you to decide which files or lines that you should mutate. You can both allow and deny resources
but it's important to know what when you add a filter all files will be ignored as default and you have to allow them.

For example: 

```c#
    "Filter":  {
                   "FilterItems":  [
                                     {
                                           "Effect":  "Allow",
                                         "Resource":  "*/src/some/files.cs",
                                          "Lines":  [
                                                       "59,10"
                                                    ]
                                       },
                                       {
                                         "Effect":  "Allow",
                                         "Resource":  "*SuperFile*",
                                       },
                                       {
                                           "Effect":  "Deny",
                                           "Resource":  "*mock*"
                                       }
                                   ]
               },
```

In this example we will: 

- Allow */src/some/files.cs
- Allow any files that contains "SuperFile" as long as they don't contain "mock"
- All other files are ignored

We use filter a lot to run mutations on specific subsets for example new pull requests.

#### DotNetPath

If you use dotnet as test runner and we can't find dotnet.exe automatically it is possible to set it manually with this property. 

### WPF Application 

The WPF is in a very early stage but it is possible to: 

- Create new projects 
- Create and see all the different mutations 
- Run mutations and see result details 

Simply open the Unima.exe, click new project and follow the instructions.

![Example of result](https://i.imgur.com/sAISq0h.jpg)

## Available mutations

Here is a short list of all current available mutations:

-	Conditional boundary mutations (>= , >, <=, <, etc)
-	Math mutators (+, -, *, etc) 
-	Negate conditional mutations (>, <, !=, ==, etc) 
-	Increment mutations (++, --)
-	Negate type mutation (i is bool, !(i is bool))
-	Return value mutation (return -1 instead of 0, return null instead of object, throw exception instead of returning null, etc) 

## License

This project is licensed under the MIT License. See the [LICENSE.md](LICENSE.md) file for details.

