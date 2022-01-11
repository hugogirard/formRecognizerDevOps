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

This section will explain you how to run this sample into your own Azure tenant.

## Fork the repository

The first thing you need to do is to fork this Github repository.

To do so, click the Fork button in the top right menu.

![fork](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/images/fork.png)

## Create service principal to run the Github actions

To create the resources in Azure you will need to create a service principal that will be used in the infra pipeline.  To do this please follow this [link](https://github.com/marketplace/actions/azure-login#configure-a-service-principal-with-a-secret). Be sure to take note of the credential returned when creating the **service principal** you will need it for the next step.

Because multiple resource groups will be create **don't create a scope at resource group level**.

The command should look something like this

```
az ad sp create-for-rbac --name "sp-gh-action" --role contributor --sdk-auth
```

## Create the needed Github Secrets

The pipelines need to have some Github secrets to be created before execution.  To create the secrets, click the Settings in the menu.

![settings](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/images/settings.png)

Next, click Secrets in the left menu.

![secrets](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/images/secrets.png)

Now click on the button **New repository secret**.

![newreposecret](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/images/newreposecret.png)

Now create all the following secrets

| Name | Value
| ----- | -----
| AZURE_CREDENTIALS | The value from the step before when creating the service principal.
| ADMIN_PRINCIPAL_OBJECT_ID | This is the ID related to an ADMIN that will have access to the key vault secrets.  This secret is optional and related to [access policies in KeyVault](https://docs.microsoft.com/en-us/azure/key-vault/general/assign-access-policy?tabs=azure-portal).
| SP_PRINCIPAL_OBJECT_ID | The clientId of the Service Principal created before
| SUBSCRIPTION_ID | The ID of the subscription

## Create the environment in Github

For the deployment to work correctly between your different environment (Dev, QA, Prod) you need to create them in Github.  For this go to Settings.

![settings](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/images/settings.png)

In the left menu click Environments

![newreposecret](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/images/env.png)

Click the new environment button

![newreposecret](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/images/newenvironment.png)

You will create DEV and QA with those settings

![settings](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/images/set1.png)

Now, create the PROD environment with the current settings. 

![settings](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/images/set2.png)

For production, you will add a reviewers, you can use your own Github user.  The goal here is most of the time before going to production you want someone to approve the deployment of the artifact, in this case the trained model tested in QA migrated from Dev.

## Run the infrastructure Github Action

Now is time to create all the Azure Resources by running the Github Action that will create the infrastructure as code.

Now, click on the menu in the **Actions** tab.

![newreposecret](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/images/actions.png)

You have a warning that tell you the Github Action are disabled, click the gree button to enable the Github actions.

![newreposecret](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/images/accept.png)

Now click on the Create Azure Resources in the left menu.

![newreposecret](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/images/createazureresource.png)

Now click on the Run workflow button.

![newreposecret](https://raw.githubusercontent.com/hugogirard/formRecognizerDevOps/main/images/run.png)

This Github action will create in parallel the 3 environments, give access to your service principal to the Azure Key Vault and build and deploy the Azure Function.