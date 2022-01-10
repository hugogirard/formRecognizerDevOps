# Introduction

The goal of this sample is to provide an end-to-end scenario or to migrate your trained custom model in Form Recognizer between your different environments using CI/CD.

This example is using Github Actions but the same can be achieved using Azure DevOps or any other CI/CD tools.

To facilitate the deployment and training of the custom model an **Azure Function** was developped.  This is not **MANDATORY**, you can do everything leveraging the Rest API of Form Recognizer and creating HTTP call in your pipeline using bash or powershell script.

The function provide easier communication access leveraging the Form Recognizer SDK.

# Architecture

![architecture](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/diagram/architecture..drawio.svg)

The sample contains 3 environments, **DEV**, **QA** and **Production**.

In development, most of the time you will train the model using the [Form Labeling tool](https://docs.microsoft.com/en-us/azure/applied-ai-services/form-recognizer/label-tool) (v2 of the API) or the [Form Recognizer Studio](https://docs.microsoft.com/en-us/azure/applied-ai-services/form-recognizer/concept-form-recognizer-studio) for the newest version.

This sample is using the most recent version of the Form Recognizer API (v3.0).  Once you model is trained using the UI tool is possible to save it in code source if needed.  

Saving your model in code source can be a good approach to keep a backup of your model in a **git** repository.  Keep in mind, **this won't save the trained model** but the document and tag and labelling needed to train your model.

In this sample, this is what it's done to train the model.

This is the flow of our CI/CD.

1) A user train the model with the assets needed calling the Azure Function. This step is not mandatory and the model can be trained using the Form Recognizer Tool.
2) The previous step will create the trained model in the Development environment, once is done the user trigger the pipeline to copy the model into QA.  Before copying the model from DEV to QA the pipeline validates if the entered ID for the model is present in DEV.  
3) If the previous step succeeds, the model is copied to QA.
4) The team is now ready to test the model in the QA environment.
5) The model is tested and ready to migrate to production, from there an approver need to approve the model to be copied from QA to Production.
6) Once approved, the model is copied from QA to Production.

# How to execute this sample

The first thing you need to do is to fork this Github repository.

To do so, click the Fork button in the top right menu.

