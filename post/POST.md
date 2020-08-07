---
title: Experimenting with RobotFramework and Selenium
published: false
description: A walkthrough a C# web app tested with Nunit, Selenium, and RobotFramework.
tags: ["beginners", "testing", "selenium", "robotframework"]
---

## Selenium, NUnit, and Robot Framework Walkthrough

Let me start by saying, I'm not a frontend developer by no stretch of the imagination.</br>
I don't do web pages, or UI's...</br>
:smirk:

But...</br>
As clearly stated in my profile (if it still there by the time you read this) `I'm a development ecosystem enthusiast...`</br>
And I felt very enthusiastic about working with [Selenium](https://www.selenium.dev/) and [Robot Framework](https://robotframework.org).

The real story is, I was waiting for an opportunity to try out [Selenium](https://www.selenium.dev/),</br>
as a backend developer, these type of opportunities come few and far between.</br>
That is until one day, I bumped into [Robot Framework](https://robotframework.org),</br>
and I've noticed the framework has a [SeleniumLibrary](https://robotframework.org/SeleniumLibrary/)...</br>

:bell: :bell: :bell: That rang my *two-for-the-price-of-one bell*! :grin:

So I decided to give it a whirl... And then I thought...</br>
Well, lately I've been writing mostly *Java*,</br>
maybe this is also an opportunity to brush up on my *dotnet* and *C#* skills.</br>
:eyeglasses:

So...</br>
To make a long story short (if that's still possible),</br>
Here's what I came up with, hope you'll enjoy it.

### Walkthrough

This is an example C# WebApp tested using [Selenium](https://www.selenium.dev/)
browser automation with [Nunit](https://nunit.org/) testing framework for unit tests</br>
and [Robot Framework](https://robotframework.org) automation framework for acceptance tests.

#### Base Requirements

- [.Net Core > 3](https://dotnet.microsoft.com/download/dotnet-core/3.1) - written with `.Net Core 3.1.102`.
- [Python > 3](https://www.python.org/downloads/) - written with `Python 3.8.2` (used for the acceptance tests).

Before starting, please clone [this repository][0] and step into the solution folder.</br>
It will be easier to follow this tutorial, as the next steps will assume you have done so:

```shell
git clone https://github.com/TomerFi/selenium-nunit-robotframework-tutorial.git
cd selenium-nunit-robotframework-tutorial
```

#### Web Application

In `src/DemoWebApp`, you'll find a simple `C# WebApp` created from the **basic template provided with** `dotnet`.</br>
On top of the base application I've added:

- a `button` tag with the id **clickmeButton**.
- a `h2` tag with the id **displayHeader** and the initial content text of **Not clicked**.
- a `script function` called **clickButton** invoked by a click event from the **clickmeButton**</br>
    and changes the **displayHeader**'s content text to **Button clicked**.

I've added these elements in `src/DemoWebApp/Pages/Index.cshtml`.</br>
Other than that I've changed nothing from the basic template:

```html
@page
@model IndexModel
@{
    ViewData["Title"] = "Home page";
}

<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <p>Learn about <a href="https://docs.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
    <button id="clickmeButton" type="button" onclick="clickButton()">Click Me!</button>
    <h2 id="displayHeader">Not clicked</h2>
</div>

<script>
function clickButton() {
    document.getElementById("displayHeader").textContent = "Button clicked";
}
</script>
```

To run the project and serve the app at `http://localhost:5000`:

```shell
dotnet run -p src/DemoWebApp
```

#### Unit Testing

In `tests/DemoWebApp.Tests`, you'll find the test project for performing unit tests, I've used:

- [Nunit](https://nunit.org/) as the testing framework for the project.
- [Selenium](https://www.selenium.dev/) as the toolset providing browser capabilities and automation.

`tests/DemoWebApp.Tests/DemoWebAppTest.cs` is test class, `Nunit` will pick it up based on its name:

I've used the `OneTimeSetup` attribute to spin-up the server before executing the test cases:

```csharp
[OneTimeSetUp]
public void SetUpWebApp()
{
    app = DemoWebApp.Program.CreateHostBuilder(new string[] { }).Build();
    app.RunAsync();
}
```

And the `OneTimeTearDown` attribute to shutdown the server afterward.

```csharp
[OneTimeTearDown]
public void TearDownWebApp()
{
    app.StopAsync();
    app.WaitForShutdown();
}
```

The test itself is pretty straightforward:

- It first navigates to the server at `http://localhost:5000`.
- It will then find the `button` element by its id and click it.
- Finally, it will make sure the `h2` element's content text is `Button clicked`.

The assert statement evaluates the `clicked` boolean value,</br>
which will be false if expected test conditions are not met within 10 seconds.

```csharp
public void TestButtonClick(Type drvType)
{
    bool clicked;
    using (var driver = (IWebDriver)Activator.CreateInstance(drvType))
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        driver.Navigate().GoToUrl("http://localhost:5000");
        driver.FindElement(By.Id("clickmeButton")).Click();

        clicked = wait.Until(ExpectedConditions.TextToBePresentInElement(
            driver.FindElement(By.Id("displayHeader")), "Button clicked"));
    }
    Assert.True(clicked, "button not clicked.");
}
```

The test-cases will invoke the `TestButtonClick` test 3 times, one for each `TestCase`.</br>
The result will be 3 tests performed, one with the `chrome` driver, one with the `firefox` driver,
and one with the `ie` driver.

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

#### Acceptance Testing

For acceptance tests I've used:

- [Robot Framework](https://robotframework.org) as the automation framework for executing the tests.
- [SeleniumLibrary](https://robotframework.org/SeleniumLibrary/) as the library providing browser capabilities and automation.

For the next steps, step into the `acceptance` folder.</br>
The acceptance tests don't have, nor should it have, any direct</br>
connection to the solution's base code.

##### Prepare Environment

`Robot Framework` is a python tool, it requires a python binary and some requirements.</br>
Assuming you have [Python](https://www.python.org/downloads/) installed, and you're in the `acceptance` folder,</br>
Just do:

```shell
pip install --upgrade -r requirements.txt
```

As this is the acceptance tests part, **the tests need a web server serving the web app.**</br>
You can follow the [Web Application section](#web-application) to run the web app locally, or run it as you see fit.</br>
just **don't forget** to set the `URL` variable in `acceptance/resources.robot` to the correct address:

```robotframework
${URL}              http://localhost:5000
```

> Please note, *markdown* doesn't support *robotframework* syntax highlighting natively.</br>
> There's another version of this tutorial in *restructuredText* which supports *robotframework* syntax highlighting
> [here](https://github.com/TomerFi/selenium-nunit-robotframework-tutorial/blob/master/post/ORIGINAL.rst).

##### Drivers

You can download the drivers stored in `acceptance/drivers` with the following links.</br>
Just mind the versions and make sure they're in conjunction with the versions used in
`tests/DemoWebApp.Tests/DemoWebApp.Tests.csproj`:

```xml
<ItemGroup>
    <PackageReference Include="Selenium.WebDriver.ChromeDriver" Version="83.0.4103.3900" />
    <PackageReference Include="Selenium.WebDriver.IEDriver" Version="3.150.1" />
    <PackageReference Include="Selenium.Firefox.WebDriver" Version="0.26.0" />
</ItemGroup>
```

To save you the troubles, here are the links for the drivers download:

- [Chrome Driver](https://chromedriver.chromium.org/downloads)
- [Internet Explorer Driver](https://www.selenium.dev/downloads/)
- [Firefox Driver](https://github.com/mozilla/geckodriver/releases)

##### Tests

`acceptance/webapp_tests.robot` is the `test suite`.</br>
It declares 3 `Test Cases`, one for each driver.</br>
Each test-case uses `Test Template` with its own `Browser` and `Executable` arguments.</br>
**Make sure the driver executables are in the correct path**.

```robotframework
*** Settings ***
...
Test Template    Press Button

*** Test Cases ***             Browser    Executable
Test With Chrome               chrome     drivers/chromedriver
Test With Internet Explorer    ie         drivers/iedriver
Test With Firefox              firefox    drivers/geckodriver
```

The `Test Template` invokes the keyword named `Press Button`,</br>
For each execution, what `Press Button` does is pretty self-explanatory by its `BDD` nature:

```robotframework
*** Keywords ***
Press Button
    [Arguments]    ${browser}    ${executable}
    Open Browser With Url    ${browser}    ${executable}
    Click Test Button
    Validate New Text
    [Teardown]    Close Browser
```

The result of running this test suite will be 3 tests, one for each driver,
each pressing the button and validating the side effects.

The `Press Button` uses 4 other keywords to perform its action.</br>
As you can see in the `Settings` section, I've declared `acceptance/resources.robot` as a resource.</br>
It provides us with the following custom keywords:

- `Open Browser With Url`
- `Click Test Button`
- `Validate New Text`

The 4th keyword, `Close Browser`, is not a custom one, it comes from [SeleniumLibrary](https://robotframework.org/SeleniumLibrary/),
imported within `acceptance/resources.robot`:

```robotframework
*** Settings ***
...
Library          SeleniumLibrary
```

The same library is also used in by the custom keywords in `acceptance/resources.robot`.

To execute the acceptance tests, simply run:

```shell
robot -d rfoutput webapp_tests.robot
```

This will run the tests and save a pretty and useful HTML report summary and xml logs in a folder called `rfoutput` (gitignored).</br>
You can see an example of the summary report [here](https://robotframework.org/robotframework/latest/RobotFrameworkUserGuide.html#report-file).

### Links

- [GitHub repository][0]
- [Nunit3 home](https://nunit.org/)
- [Nunit3 docs](https://github.com/nunit/docs/wiki)
- [Selenium home](https://www.selenium.dev/)
- [Selenium docs](https://www.selenium.dev/documentation/en/)
- [Robot Framework home](https://robotframework.org)
- [Robot Framework docs](http://robotframework.org/robotframework/latest/RobotFrameworkUserGuide.html)
- [SeleniumLibrary home](https://robotframework.org/SeleniumLibrary/)
- [SeleniumLibrary docs](https://robotframework.org/SeleniumLibrary/SeleniumLibrary.html)

**:wave: See you in the next tutorial :wave:**

[0]: https://github.com/TomerFi/selenium-nunit-robotframework-tutorial
