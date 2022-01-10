# Introduction

The goal of this sample is to provide an end-to-end scenario or to migrate your trained custom model in Form Recognizer between your different environments using CI/CD.

This example is using Github Actions but the same can be achieved using Azure DevOps or any other CI/CD tools.

To facilitate the deployment and training of the custom model an **Azure Function** was developped.  This is not **MANDATORY**, you can do everything leveraging the Rest API of Form Recognizer and creating HTTP call in your pipeline using bash or powershell script.

The function provide easier communication access leveraging the Form Recognizer SDK.

# Architecture

![test](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/diagram/architecture..drawio.svg)