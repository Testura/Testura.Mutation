![Testura Logo](https://i.imgur.com/MC35ew9.png)

Are you looking for ways to improve the quality and test coverage of your unit tests? Then Testura and mutation testing may be something for you.

Testura is a mutation testing tool/visual studio extension for C# that verifies the quality of your unit tests by injecting different mutations in your production code and then checks whether your unit tests catch them. If your unit tests: 

- Fail it means that your tests found the mutation and you have good coverage.
- Pass it means that the mutant survived and you do not have sufficient coverage of the specific functionality.

![Example of mutation](https://i.imgur.com/ZFPbEyI.png)
*Example of a mutation* 

## Why should I use Testura?
 
- Quick and easy to set up.
- Very flexible and it's easy to set up which files and lines you want to mutate.
- Can take your test coverage from good to great.
- Can use it directly in visual studio (by extension) or in pipline (with console application)

# Install

-

## Usage

### Visual studio extension 

![Mutation explorer](https://i.imgur.com/rH8Kj8v.png)

#### Configuration

The first thing you need to do is to configure the extension at `Extensions > Testura.Mutation > Config..`.

#### Create mutations 

You can run mutations in three different ways: 

- Go to `Extensions > Testura.Mutation > Mutate solution..` to mutate the whole solution (except for ignored/test projects).
- Right-click on solution, project, directory or files in the solution explorer and pick `Mutate files...`
- Go to a file, select lines that you want to mutate and then right-lick and pick `Mutate lines`

![Mutatate lines](https://i.imgur.com/9Cjma94.png)

It may take a while to create your mutations depending on number of files/lines.

When they are done you see how the mutation look by clicking the small blue button the the right or doubble click and hover over the statemen (if you have highlight on).

### Run mutations

Click the play icon in the mutation explorer to run your mutations. The mutations will either: 

- Survive (red icon) which means that you don't have unit test that cover this mutation.
- Die (green icon) which means that your unit test found the mutation.
- Finish with unknown error (purple icon), this are usually because of compilation error (bad mutation).


### Highlights 

![Mutatate highlights](https://i.imgur.com/BSKxi7R.png)

### Debug 

Please check the output window.

### Console application 

![Console application](https://i.imgur.com/KlWLGID.png)

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
path\to\Tetura.Mutation.Console.exe local --configPath "path/To/Json/Config" --outputPath "path/to/output/directory"
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
			"MyTestProject"
                         "*.Tests"
                     ],
```

#### BuildConfiguration

If we should mutate "Debug" or "Release" (it's important to build your project before trying to run Testura)

#### TestRunner

Testura offers three different flags for the test runner: 

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

Filter is a way for you to decide which files or lines that Testura will mutate. You can both allow and deny resources,
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

In this example, Testura will: 

- Allow */src/some/files.cs but only line 59 to 69
- Allow any files that contains "SuperFile" as long as they don't contain "mock"
- All other files are ignored

We use filter a lot to run mutation operators on specific subsets for example new pull requests.

#### DotNetPath

If you use dotnet as the test runner and Testura can't find dotnet.exe automatically it is possible to set it manually with this property.

#### Mutators

A list of all the mutators that you want to run (look further below for tags and the default ones if you don't specifiy any mutators).

```c#
"Mutators": [ "Increment" ]
```

#### Git

Possible git values:

```c#
    "Git":  {
                   "RepositoryUrl": "url/to/your/repository",
		   "LocalPath": "where/we/should/save/repository"
		   "Branch": "master",
		   "Username": "Your username if any",
		   "Passsword": "Your password if any"
		   "ForceClone": false, 
		   "GenerateFilterFromDiffWithMaster": false
               },
```

- `RepositoryUrl`: Url to your repository 
- `LocalPath`: Path to your local directory 
- `Branch`: Name of the branch you want to clone/checkout 
- `Username/password`: If your repoistory requires username/password (are optional) 
- `ForceClone`: We always start by looking if the project exist before cloning - but if this is true we delete the local directory and clone again.
- `GenerateFilterFromDiffWithMaster`: If this is true we will look at the changes between the branch and master and generate filter items for the changes. For example if you have changed line 51 in the file "Test.cs" we will create a filter item like `{ Effect: "Allow", "Resource": "*Test.cs", "Lines": [ "51" ]`

#### Target framework 

It's possible to specifiy which target framework we should use when building the project like this: 

```c#
    "TargetFramework":  {
                   "Name": "Net47",
		   "IgnoreProjectsWithWrongTargetFramework": false
               },
```

`IgnoreProjectsWithWrongTargetFramework` is optional (default false) but is any easy way to filter out projects that doesn't match the expected target framework.

### Mutation run logger

A list of specific run loggers. For example: 

```c#
 "MutationRunLoggers": [
                       "Azure"
       ]
```

Current run loggers: 

- Azure: This logger will log progress to azure devops/VSO. Example of log line: 

```2019-05-16 14:50:29,251: Testura.Mutation.Core.Loggers.AzureMutationRunLogger: ##vso[task.setprogress value=67;]Mutation execution progress```

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
