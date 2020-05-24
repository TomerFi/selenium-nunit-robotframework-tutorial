*** Settings ***
Documentation    A resource files with reusable keywords and variables.
Library          SeleniumLibrary

*** Variables ***
${URL}              http://localhost:5000
${DELAY}            0
${BUTTON_ID}        clickmeButton
${HEADER_ID}        displayHeader
${ORIGINAL_TEXT}    Not clicked
${NEW_TEXT}         Button clicked

*** Keywords ***
Open Browser With Url
    [Arguments]    ${browser}    ${executable}
    Open Browser    ${URL}    ${browser}    executable_path=${executable}
    Set Selenium Speed    ${DELAY}
    Validate Original Text

Validate Original Text
    Element Text Should Be    id:${HEADER_ID}    ${ORIGINAL_TEXT}

Click Test Button
    Click Button              id:${BUTTON_ID}

Validate New Text
    Element Text Should Be    id:${HEADER_ID}    ${NEW_TEXT}
