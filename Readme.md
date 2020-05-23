# Example C# WebApp Testing with Nunit and Selenium

**Build dotnet core > 3.**

Example of a simple c# WebApp (created from the basic template).</br>
The base WebApp was added with a button, an h2 header and a script that change the h2 text content when the button is clicked.</br>
Added in [/DemoWebApp/Pages/Index.cshtml](/DemoWebApp/Pages/Index.cshtml)

In the testing area, we have a test fixture starting our app at `localhost:5000` and stopping it when the tests are done,</br>
And a test made from a couple of test cases (one for each browser).</br>

The test uses selenium to browse our app, click the button and validate the text has changed.

To check it out, just clone it and run the tests:

```shell
dotnet test
```

## Literature

- [Nunit3 home](https://nunit.org/)
- [Nunit3 docs](https://github.com/nunit/docs/wiki)
- [Selenium home](https://www.selenium.dev/)
- [Selenium docs](https://www.selenium.dev/documentation/en/)

> Please Note: the repository name implies the use of [Robot Framework](https://robotframework.org).</br>
> Hopefully a usage example of [Robot Framework](https://robotframework.org) as an automation platform will be added shortly.
