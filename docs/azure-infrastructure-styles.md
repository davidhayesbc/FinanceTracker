# Azure Infrastructure and Deployment Standards

This document outlines conventions for Azure infrastructure, Bicep templates, `azure.yaml` configurations, and deployment processes within the FinanceTracker project.

## General Azure Principles

- **Resource Naming:** (Define your Azure resource naming conventions here. Consider using prefixes, suffixes, and environment indicators.)
- **Tagging:** (Specify mandatory and recommended tags for Azure resources, e.g., `environment`, `cost-center`, `application-name`.)
- **Regions:** (Preferred Azure regions for deployment.)
- **Security:** (General security guidelines, e.g., use of Azure Key Vault, managed identities, network security groups.)

## Bicep Development

- **Modularity:** (Conventions for creating and using Bicep modules.)
- **Parameters & Variables:** (Guidelines for parameter files, use of `params` vs. `var`.)
- **Outputs:** (Standard outputs to include in Bicep templates.)
- **Naming within Bicep:** (Naming conventions for resources, parameters, variables, and outputs within Bicep files.)
- **Best Practices:** Adhere to general Bicep best practices for clarity, reusability, and maintainability.

## Azure Developer CLI (`azd`)

- **`azure.yaml` Structure:** (Conventions for defining services, hooks, and other configurations in `azure.yaml`.)
- **Service Definitions:** (How to define services, their language, host, and project paths.)
- **Hooks:** (Guidelines for using pre-package, post-package, pre-deploy, and post-deploy hooks.)
- **Environment Management:** (Conventions for managing `azd` environments.)

## Deployment

- **CI/CD:** (Outline of the CI/CD process, tools used, and any specific conventions for pipeline definitions.)
- **Secrets Management:** Secrets shoud be tored in Azure Key Vault integration.

---

_This guide should be updated as project standards evolve._
