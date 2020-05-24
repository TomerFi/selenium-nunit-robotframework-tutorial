*** Settings ***
Documentation    Web Application Test Suite.
...
...              Tests for our web application.
...              Currenyly we have one test that presses the deisgnated
...              button and validates the resulting text change.
Resource         resources.robot
Test Template    Press Button

*** Test Cases ***             Browser    Executable
Test With Chrome               chrome     drivers/chromedriver
Test With Internet Explorer    ie         drivers/iedriver
Test With Firefox              firefox    drivers/geckodriver

*** Keywords ***
Press Button
    [Arguments]    ${browser}    ${executable}
    Open Browser With Url    ${browser}    ${executable}
    Click Test Button
    Validate New Text
    [Teardown]    Close Browser
