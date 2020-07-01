=============================================
Selenium, NUnit, and Robot Framework tutorial
=============================================

An example C# WebApp tested using `Selenium <https://www.selenium.dev/>`_ browser automation with
`Nunit <https://nunit.org/>`_ testing framework for unit tests and
`Robot Framework <https://robotframework.org>`_ automation framework for acceptance tests.

.. contents::
   :local:
   :depth: 2

Base Requirements
=================

* `.Net Core > 3 <https://dotnet.microsoft.com/download/dotnet-core/3.1>`_ - written with ``.Net Core 3.1.102``.
* `Python > 3 <https://www.python.org/downloads/>`_ - written with ``Python 3.8.2`` (used for the acceptance tests).

| Before starting, please clone this repository and step into the solution folder.
| It will be easier to follow this guide, as the next steps will assume you have done so:

.. code-block:: shell

   git clone https://github.com/TomerFi/selenium-nunit-robotframework-tutorial.git
   cd selenium-nunit-robotframework-tutorial

Web Application
===============

+----------------------------+
| `DemoWebApp <DemoWebApp>`_ |
+----------------------------+

| A simple ``C# WebApp`` created from the *basic template provided with* ``dotnet``.
| On top of the base application I've added:

* a ``button`` tag with the id **clickmeButton**.
* a ``h2`` tag with the id **displayHeader** and the initial content text of **Not clicked**.
* | a ``script function`` called **clickButton** invoked by a click event from the **clickmeButton**
  | and changes the **displayHeader**'s content text to **Button clicked**.

I've added these elements in `/DemoWebApp/Pages/Index.cshtml </DemoWebApp/Pages/Index.cshtml>`_.

To run the project and serve the app at ``http://localhost:5000``:

.. code-block:: shell

   dotnet run -p DemoWebApp

..

   | Please note: I want to clarify that the web application is not written by me,
   | It's the original template created with the command ``dotnet new webapp``.
   | All id did was adding the ``button`` and ``h2`` tags, and the simple script.

Unit Testing
============

+----------------------------------------+
| `DemoWebApp.Tests <DemoWebApp.Tests>`_ |
+----------------------------------------+

For unit testing I've used:

* `Nunit <https://nunit.org/>`_ as the testing framework the project.
* `Selenium <https://www.selenium.dev/>`_ as the toolset providing browser capbalities and automation.

`DemoWebAppTest.cs <DemoWebApp.Tests/DemoWebAppTest.cs>`_ is test class, ``Nunit`` will pick it
based on its name.

I've used the ``OneTimeSetup`` attribute to spin-up the server prior of executing the test cases:

.. code-block:: csharp

   [OneTimeSetUp]
   public void SetUpWebApp()
   {
       app = DemoWebApp.Program.CreateHostBuilder(new string[] { }).Build();
       app.RunAsync();
   }

And the ``OneTimeTearDown`` attribute to shutdown the server afterwards.

.. code-block:: csharp

   [OneTimeTearDown]
   public void TearDownWebApp()
   {
       app.StopAsync();
       app.WaitForShutdown();
   }

The test itself is pretty straightforward:

* It first navigates to the server at ``http://localhost:5000``.
* It will then find the ``button`` element by its id and click it.
* Finaly, it will make sure the ``h2`` element's content text is ``Button clicked``.

| The assert statement evaluates the ``clicked`` boolean value,
| which will be false if expected test conditions are not met withing 10 seconds.

.. code-block:: csharp

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

| The test-cases will invoke the ``TestButtonClick`` test 3 times, one for each ``TestCase``.
| The result will be 3 tests performed, one with the ``chrome`` driver, one with the ``firefox`` driver,
| and one with the ``ie`` driver.

.. code-block:: csharp

   [TestCase(typeof(ChromeDriver))]
   [TestCase(typeof(FirefoxDriver))]
   [TestCase(typeof(InternetExplorerDriver))]
   public void TestButtonClick(Type drvType)
   {
       ...
   }

To check it out, just:

.. code-block:: shell

   dotnet test

..

   | Please note: Based on your personal environment,
   | ``Internet Explorer`` may require specific configruation for the test to pass.
   | If so, please follow `this <https://www.programmersought.com/article/1603471677/>`_.

Acceptance Testing
==================

+----------------------------+
| `acceptance <acceptance>`_ |
+----------------------------+

For acceptance tests I've used:

* `Robot Framework <https://robotframework.org>`_ as the automation framework for executing the tests.
* `SeleniumLibrary <https://robotframework.org/SeleniumLibrary/>`_ as the library providing browser capbalities and automation.

| For the next steps, step into the ``acceptance`` folder.
| The acceptance tests doesn't have, nor should it have, any direct connection to the project's base code.

Prepare Environment
-------------------

| ``Robot Framework`` is a python tool, it requires a python binary and some requirements.
| Assuming you have `Python <https://www.python.org/downloads/>`_ installed, and you're in the ``acceptance`` folder,
| Just do:

.. code-block:: shell

   pip install --upgrade -r requirements.txt

| As this is the acceptance tests part, **the tests needs a web server serving the web app.**
| You can follow the `Web Application section <#web-application>`_ to run the web app locally, or run it as you see fit.
| just **don't forget** to set the ``URL`` variable in `resources.robot <acceptance/resources.robot>`_ to the correct address:

.. code-block:: robotframework

   ${URL}              http://localhost:5000

Drivers
-------

| You can download the drivers stored in ``acceptance/drivers`` with the following links.
| Just mind the versions and make sure they're in conjunction with the versions used in `DemoWebApp.Tests.csproj <DemoWebApp.Tests/DemoWebApp.Tests.csproj>`_.

* `Chrome Driver <https://chromedriver.chromium.org/downloads>`_
* `Internet Explorer Driver <https://www.selenium.dev/downloads/>`_
* `Firefox Driver <https://github.com/mozilla/geckodriver/releases>`_

Tests
-----

| `webapp_tests.robot <acceptance/webapp_tests.robot>`_ is the ``test suite``. It declares 3 ``Test Cases``, one for each driver.
| Each test-case uses ``Test Template`` with its own ``Browser`` and ``Executable`` arguments.

.. code-block:: robotframework

   *** Settings ***
   ...
   Test Template    Press Button

   *** Test Cases ***             Browser    Executable
   Test With Chrome               chrome     drivers/chromedriver
   Test With Internet Explorer    ie         drivers/iedriver
   Test With Firefox              firefox    drivers/geckodriver

| The ``Test Template`` invokes the keyword named ``Press Button``,
| For each execution, what ``Press Button`` does is pretty self-explanatory by its ``BDD`` nature:

.. code-block:: robotframework

   *** Keywords ***
   Press Button
       [Arguments]    ${browser}    ${executable}
       Open Browser With Url    ${browser}    ${executable}
       Click Test Button
       Validate New Text
       [Teardown]    Close Browser

| The result of runing this test suite will be 3 tests, one for each driver,
| each pressing the button and validating the side effects.

| The ``Press Button`` uses 4 other keywords to perform its action.
| As you can see in the ``Settings`` section, we declare `resources.robot <acceptance/resources.robot>`_ as a resource.
| It provides us with the following custom keywords:

* ``Open Browser With Url``
* ``Click Test Button``
* ``Validate New Text``

| The 4th keyword, ``Close Browser``, is not a custom one, it comes from `SeleniumLibrary <https://robotframework.org/SeleniumLibrary/>`_,
| imported within our `resources.robot <acceptance/resources.robot>`_:

.. code-block:: robotframework

   *** Settings ***
   ...
   Library          SeleniumLibrary

The same library is also used in `resources.robot <acceptance/resources.robot>`_ by the custom keyowrds.

To execute the acceptance tests, simplly run:

.. code-block:: shell

   robot -d rfoutput webapp_tests.robot

| This will run the tests and save a pretty and useful html report summary and xml logs in a folder
| called ``rfoutput`` (gitignored). You can see and example of the summary report `here <https://robotframework.org/robotframework/latest/RobotFrameworkUserGuide.html#report-file>`_.

Links
=====

* `Nunit3 home <https://nunit.org/>`_
* `Nunit3 docs <https://github.com/nunit/docs/wiki>`_
* `Selenium home <https://www.selenium.dev/>`_
* `Selenium docs <https://www.selenium.dev/documentation/en/>`_
* `Robot Framework home <https://robotframework.org>`_
* `Robot Framework docs <http://robotframework.org/robotframework/latest/RobotFrameworkUserGuide.html>`_
* `SeleniumLibrary home <https://robotframework.org/SeleniumLibrary/>`_
* `SeleniumLibrary docs <https://robotframework.org/SeleniumLibrary/SeleniumLibrary.html>`_
