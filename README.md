# Example C# WebApp Unit Tests and Acceptance Tests

- [Base Requirements](#base-requirements)
- [Web Application](#web-application)
- [Unit Testing](#unit-testing)
- [Acceptance Testing](#acceptance-testing)
  - [Prepare Environment](#prepare-environment)
  - [Tests](#tests)
- [Links](#literature)

----------------------------------------

## Base Requirements

- [.Net Core > 3](https://dotnet.microsoft.com/download/dotnet-core/3.1) - written with `.Net Core 3.1.102`.
- [Python > 3](https://www.python.org/downloads/) - written with `Python 3.8.2` (Python is needed for the acceptance tests).

Before starting, it's advised to first clone this repository and step into the project folder,</br>
So you can execute the tests. The next steps will assume these tasks are done:

```shell
git clone https://github.com/BynetDevTeam/cs-selenium-robotframework.git
cd cs-selenium-robotframework
```

----------------------------------------

## Web Application

| [DemoWebApp](DemoWebApp) |

A simple `C# WebApp` created from the *basic template provided with `dotnet`*.</br>
The base application was added with:

- a `button` tag with the id *clickmeButton*.
- a `h2` tag with the id *displayHeader* and the initial content text of *Not clicked*.
- a `script function` called *clickButton* which is launched at a click event from the *clickmeButton*</br>
  and changes the *displayHeader*'s content text to *Button clicked*.

All of those elements were added in [/DemoWebApp/Pages/Index.cshtml](/DemoWebApp/Pages/Index.cshtml).</br>

To run the project and serve the app at `http://localhost:5000`:

```shell
dotnet run -p DemoWebApp
```

----------------------------------------

## Unit Testing

| [DemoWebApp.Tests](DemoWebApp.Tests) |

For our unit tests we used:

- [**Nunit**](https://nunit.org/) as the testing framework for our `dotnet` project.
- [**Selenium**](https://www.selenium.dev/) as the toolset providing tools for testing browser capbalities.

[**DemoWebAppTest.cs**](DemoWebApp.Tests/DemoWebAppTest.cs) is out test file,
`Nunit` will pick it based on its name and attributes.

We use the `OneTimeSetup` attributeto to spin-up our server prior to executing our tests:</br>

```csharp
[OneTimeSetUp]
public void SetUpWebApp()
{
    app = DemoWebApp.Program.CreateHostBuilder(new string[] {}).Build();
    app.RunAsync();
}
```

And the `OneTimeTearDown` attribute to shut-down our server.

```csharp
[OneTimeTearDown]
public void TearDownWebApp()
{
    app.StopAsync();
}
```

Our test is quite simple:

- It first navigates to our server at `http://localhost:5000`.
- It then find out `button` element by its id and click it.
- It then validates the `h2` element's content text to `Button clicked`.

We assert base on the `clicked` boolean value,
which will be false if our expected conditions are not met withing 10 seconds.

```csharp
public void TestButtonClick(Type drvType)
{
    bool clicked;
    using (var driver = (IWebDriver) Activator.CreateInstance(drvType))
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        driver.Navigate().GoToUrl("http://localhost:5000");
        driver.FindElement(By.Id("clickmeButton")).Click();

        clicked = wait.Until(ExpectedConditions.TextToBePresentInElement(
            driver.FindElement(By.Id("displayHeader")), "Button clicked"));
    }
    Assert.True(clicked, "button not clicked.");
}
```

In this case we designed our test-cases using attributes,</br>
The follwing will run our `TestButtonClick` test **3** times, one for each `TestCase`.</br>
The result will of course be performing 3 tests, 1 with the `chrome` driver,
one with the `firefox` driver and one with the `ie` driver.

```csharp
[TestCase(typeof(ChromeDriver))]
[TestCase(typeof(FirefoxDriver))]
[TestCase(typeof(InternetExplorerDriver))]
public void TestButtonClick(Type drvType)
{
    ...
}
```

To check it out, just:

```shell
dotnet test
```

> Please note: Based on your personal environment, `Internet Explorer` might require specific configruation for the test to pass.</br>
> If so, it's simple, please follow [this](http://www.programmersought.com/article/1603471677/).

----------------------------------------

## Acceptance Testing

| [acceptance](acceptance) |

For our acceptance tests we used:

- [**Robot Framework**](https://robotframework.org) as the automation tool for executing our tests.
- [**SeleniumLibrary**](https://robotframework.org/SeleniumLibrary/) as the library providing tools for testing browser capbalities.

Please step into the `acceptance` folder, our next steps will be executed from it as our acceptance tests doesn't have,</br>
nor should it have, any direct connection to our project base code.

### Prepare Environment

`Robot Framework` is a python tool, it requires a python binary and some requirements.</br>
Assuming you have [Python](https://www.python.org/downloads/) installed, and you're in the `acceptance` folder,</br>
Just do:

```shell
pip install --upgrade -r requirements.txt
```

As this is the acceptance tests part, our web app needs to be served somewhere.</br>
You can follow the [Web Application section](#web-application) to run our web app locally.</br>
Or you can of course run it as you see fit.</br>
just **don't forget** to set the `URL` variable in [resources.robot](acceptance/resources.robot) to the correct address:

```text
${URL}              http://localhost:5000
```

### Tests

[webapp_tests.robot](acceptance/webapp_tests.robot) is our `test suite`. We have 3 `Test Cases`,
one for each driver.</br>
Each test-case uses our `Test Template` with its own `Browser` and `Executable` arguments.

```text
*** Settings ***
...
Test Template    Press Button

*** Test Cases ***             Browser    Executable
Test With Chrome               chrome     drivers/chromedriver
Test With Internet Explorer    ie         drivers/iedriver
Test With Firefox              firefox    drivers/geckodriver
```

Our `Test Template` actually calls our `Keyword` named `Press Button`,</br>
For each execution, what `Press Button` does is pretty self-explanatory by its `BDD` nature:

```text
*** Keywords ***
Press Button
    [Arguments]    ${browser}    ${executable}
    Open Browser With Url    ${browser}    ${executable}
    Click Test Button
    Validate New Text
    [Teardown]    Close Browser
```

The result of runing this test suite will be 3 tests, 1 for each driver,</br>
each pressing the button and validating the side effects.</br>

The `Press Button` actually uses 4 other keywords to accomplish its goal.</br>
As you can see in the `Settings` section, we declare [resources.robot](acceptance/resources.robot) as a resource.</br>
It provides us with the following custom `Keywords`:

- Open Browser With Url
- Click Test Button
- Validate New Text

The 4th `Keyword`, `Close Browser`, is not a custom one, it actually comes from [SeleniumLibrary](https://robotframework.org/SeleniumLibrary/),</br>
which is imported within our [resources.robot](acceptance/resources.robot):

```text
*** Settings ***
...
Library          SeleniumLibrary
```

To execute our acceptance tests, simplly run:

```shell
robot -d rfoutput webapp_tests.robot
```

This will run our tests and save a pretty and useful html report summary and xml logs in a folder called `rfoutput`.

----------------------------------------

## Links

- [Nunit3 home](https://nunit.org/)
- [Nunit3 docs](https://github.com/nunit/docs/wiki)
- [Selenium home](https://www.selenium.dev/)
- [Selenium docs](https://www.selenium.dev/documentation/en/)
- [Robot Framework home](https://robotframework.org)
- [Robot Framework docs](http://robotframework.org/robotframework/latest/RobotFrameworkUserGuide.html)
- [SeleniumLibrary home](https://robotframework.org/SeleniumLibrary/)
- [SeleniumLibrary docs](https://robotframework.org/SeleniumLibrary/SeleniumLibrary.html)
