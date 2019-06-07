![Unima Logo](https://i.imgur.com/ELDUHai.png)

Are you looking for ways to improve the quality and test coverage of your unit tests? Then Unima and mutation testing may be something for you.

Unima is a mutation testing tool for C# that verifies the quality of your unit tests by injecting different mutations in your production code and then checks whether your unit tests catch them. If your unit tests: 

- Fail it mean that your tests found the mutation and you have good coverage.
- Pass it means that the mutant survived and you do not have sufficient coverage of the specific functionality.

![Example of mutation](https://i.imgur.com/ZFPbEyI.png)
*Example of a mutation* 

## Why should I use Unima?
 
- Quick and easy to set up.
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

1. Run a baseline (simply execute all your unit tests to make sure that everything is fine).
2. Apply mutation operators to generate mutants.
3. Go through each mutant and run all unit tests (we run all unit tests for each mutant).

After the unit test execution, you will get a couple of different result files (we plan to develop a filtering function in the future):

- A trx file 
- A markdown file 
- A short summary in txt
- A html file 
- A json file

The result files contain your mutation score (number of killed mutants divided by the total number of mutants) as well as details about all survived mutants.

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

If we should mutate "Debug" or "Release" (it's important to build your project before trying to run Unima!)

#### TestRunner

Unima offers three different flags for the test runner: 

- dotnet: Supports all test frameworks and .NET Core 
- nunit: Supports nunit non-core 
- xunit: Support xunit non-core 

In most cases it's best to run "dotnet".

#### NumberOfTestRunInstances

We run all unit tests for all mutants and this property tells us how many sessions we should run in parallel.

#### IgnoredProjects

List of projects that we shouldn't mutate or run tests from. For example: 
```c#
    "IgnoredProjects":  [
                            "Name.Of.The.Project"
                        ],
```
#### Filter

Filter is a way for you to decide which files or lines that Unima will mutate. You can both allow and deny resources,
but it's important to know that when you add a filter, all files will be ignored by default and you have to allow them.

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

In this example, Unima will: 

- Allow */src/some/files.cs but only line 59 to 69
- Allow any files that contains "SuperFile" as long as they don't contain "mock"
- All other files are ignored

We use filter a lot to run mutation operators on specific subsets for example new pull requests.

#### DotNetPath

If you use dotnet as the test runner and Unima can't find dotnet.exe automatically it is possible to set it manually with this property.

#### Mutators

A list of all the mutators that you want to run (look further below for tags and the default ones if you don't specifiy any mutators).

```c#
"Mutators": [ "Increment" ]
```

### Mutation run logger

A list of specific run loggers. For example: 

```c#
 "MutationRunLoggers": [
                       "Azure"
       ]
```

Current run loggers: 

- Azure: This logger will log progress to azure devops/VSO. Example of log line: 

```2019-05-16 14:50:29,251: Unima.Core.Loggers.AzureMutationRunLogger: ##vso[task.setprogress value=67;]Mutation execution progress```

### WPF Application 

The WPF is in a very early stage but it is possible to: 

- Create new projects 
- Create and see all the different mutation operators
- Apply mutation operators and see result details 

Simply run Unima.exe, click new project and follow the instructions.

![Example of result](https://i.imgur.com/sAISq0h.jpg)

## Available mutation operators

Here is a short list of all currently available mutation operators:

| Name                 | Mutations                                                                                             | Tag                   | Default |
|----------------------|-------------------------------------------------------------------------------------------------------|-----------------------|---------|
| Conditional boundary | >= , >, <=, <, etc                                                                                    | ConditionalBoundary   | true    |
| Math                 | +, -, *, etc                                                                                          | Math                  | true    |
| Increment            | ++, --                                                                                                | Increment             | true    |
| Negate conditional   | >, <, !=, ==, etc                                                                                     | NegateCondtional      | true    |
| Negate type          | i is bool, !(i is bool)                                                                               | NegateTypeCompability | true    |
| Return value         | return -1 instead of 0, return null instead of object, throw exception instead of returning null, etc | ReturnValue           | true    |
| Void Method          | Remove all calls to void methods                                                                      | MethodCall            | false   |

## License

This project is licensed under the MIT License. See the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgements

[![Web site](https://www.testomatproject.eu/wp-content/uploads/2017/11/TESTOMAT_Logo_300.png)](https://www.testomatproject.eu/)

This work has been financially supported by the ITEA3 initiative TESTOMATProject through Vinnova - Sweden's innovation agency.
