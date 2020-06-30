=================================================
Example C# WebApp Unit Tests and Acceptance Tests
=================================================

.. contents::
   :local:
   :depth: 2

Base Requirements
=================

* `.Net Core > 3 <https://dotnet.microsoft.com/download/dotnet-core/3.1>`_ - written with ``.Net Core 3.1.102``.
* `Python > 3 <https://www.python.org/downloads/>`_ - written with ``Python 3.8.2`` (Python is needed for the acceptance tests).

| Before starting, it's advised to first clone this repository and step into the project folder,
| So you can execute the tests. The next steps will assume these tasks are done:

.. code-block:: shell

   git clone https://github.com/TomerFi/selenium-robotframework-example.git
   cd selenium-robotframework-example

Web Application
===============

+----------------------------+
| `DemoWebApp <DemoWebApp>`_ |
+----------------------------+

| A simple ``C# WebApp`` created from the *basic template provided with* ``dotnet``.
| The base application was added with:

* a ``button`` tag with the id *clickmeButton*.
* a ``h2`` tag with the id *displayHeader* and the initial content text of *Not clicked*.
* | a ``script function`` called *clickButton* which is launched at a click event from the *clickmeButton*
  | and changes the *displayHeader*'s content text to *Button clicked*.

All of those elements were added in `/DemoWebApp/Pages/Index.cshtml </DemoWebApp/Pages/Index.cshtml>`_.

To run the project and serve the app at ``http://localhost:5000``:

.. code-block:: shell

   dotnet run -p DemoWebApp


Unit Testing
============

+----------------------------------------+
| `DemoWebApp.Tests <DemoWebApp.Tests>`_ |
+----------------------------------------+

For our unit tests we used:

* `Nunit <https://nunit.org/>`_ as the testing framework for our ``dotnet`` project.
* `Selenium <https://www.selenium.dev/>`_ as the toolset providing tools for testing browser capbalities.

`DemoWebAppTest.cs <DemoWebApp.Tests/DemoWebAppTest.cs>`_ is our test file,
``Nunit`` will pick it based on its name and attributes.

We use the ``OneTimeSetup`` attributeto to spin-up our server prior to executing our tests:

.. code-block:: csharp

   [OneTimeSetUp]
   public void SetUpWebApp()
   {
       app = DemoWebApp.Program.CreateHostBuilder(new string[] {}).Build();
       app.RunAsync();
   }

And the ``OneTimeTearDown`` attribute to shutdown our server.

.. code-block:: csharp

   [OneTimeTearDown]
   public void TearDownWebApp()
   {
       app.StopAsync();
       app.WaitForShutdown();
   }

Our test is quite simple:

* It first navigates to our server at ``http://localhost:5000``.
* It then finds our ``button`` element by its id and clicks it.
* It then validates the ``h2`` element's content text to ``Button clicked``.

We assert base on the ``clicked`` boolean value,
which will be false if our expected conditions are not met withing 10 seconds.

.. code-block:: csharp

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

| In this case we designed our test-cases using attributes,
| The follwing will run our ``TestButtonClick`` test **3** times, one for each ``TestCase``.
| The result will of course be performing 3 tests, 1 with the ``chrome`` driver,
| one with the ``firefox`` driver and one with the ``ie`` driver.

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

   | Please note: Based on your personal environment, ``Internet Explorer`` might require specific configruation for the test to pass.
   | If so, it's simple, please follow `this <http://www.programmersought.com/article/1603471677/>`_.

Acceptance Testing
==================

+----------------------------+
| `acceptance <acceptance>`_ |
+----------------------------+

For our acceptance tests we used:

* `Robot Framework <https://robotframework.org>`_ as the automation tool for executing our tests.
* `SeleniumLibrary <https://robotframework.org/SeleniumLibrary/>`_ as the library providing tools for testing browser capbalities.

| Please step into the ``acceptance`` folder, our next steps will be executed from it as our
| acceptance tests doesn't have, nor should it have, any direct connection to our project base code.

Prepare Environment
-------------------

| ``Robot Framework`` is a python tool, it requires a python binary and some requirements.
| Assuming you have `Python <https://www.python.org/downloads/>`_ installed, and you're in the ``acceptance`` folder,
| Just do:

.. code-block:: shell

   pip install --upgrade -r requirements.txt

| As this is the acceptance tests part, our web app needs to be served somewhere.
| You can follow the `Web Application section <#web-application>`_ to run our web app locally.
| Or you can of course run it as you see fit.
| just **don't forget** to set the ``URL`` variable in `resources.robot <acceptance/resources.robot>`_ to the correct address:

.. code-block:: robotframework

   ${URL}              http://localhost:5000

Tests
-----

| `webapp_tests.robot <acceptance/webapp_tests.robot>`_ is our ``test suite``. We have 3 ``Test Cases``, one for each driver.
| Each test-case uses our ``Test Template`` with its own ``Browser`` and ``Executable`` arguments.

.. code-block:: robotframework

   *** Settings ***
   ...
   Test Template    Press Button

   *** Test Cases ***             Browser    Executable
   Test With Chrome               chrome     drivers/chromedriver
   Test With Internet Explorer    ie         drivers/iedriver
   Test With Firefox              firefox    drivers/geckodriver

| Our ``Test Template`` actually calls our ``Keyword`` named ``Press Button``,
| For each execution, what ``Press Button`` does is pretty self-explanatory by its ``BDD`` nature:

.. code-block:: robotframework

   *** Keywords ***
   Press Button
       [Arguments]    ${browser}    ${executable}
       Open Browser With Url    ${browser}    ${executable}
       Click Test Button
       Validate New Text
       [Teardown]    Close Browser

| The result of runing this test suite will be 3 tests, 1 for each driver,
| each pressing the button and validating the side effects.

| The ``Press Button`` actually uses 4 other keywords to accomplish its goal.
| As you can see in the ``Settings`` section, we declare `resources.robot <acceptance/resources.robot>`_ as a resource.
| It provides us with the following custom ``Keywords``:

* Open Browser With Url
* Click Test Button
* Validate New Text

| The 4th keyword, ``Close Browser``, is not a custom one, it actually comes from `SeleniumLibrary <https://robotframework.org/SeleniumLibrary/>`_,
| which is imported within our `resources.robot <acceptance/resources.robot>`_:

.. code-block:: robotframework

   *** Settings ***
   ...
   Library          SeleniumLibrary

To execute our acceptance tests, simplly run:

.. code-block:: shell

   robot -d rfoutput webapp_tests.robot

| This will run our tests and save a pretty and useful html report summary and xml logs in a folder
| called ``rfoutput``. You can see and example of the summary report `here <https://robotframework.org/robotframework/latest/RobotFrameworkUserGuide.html#report-file>`_.

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
