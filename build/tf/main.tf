terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>2.90.0"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "brokergroup" {
  name     = "LoanBroker"
  location = var.Location
}

resource "azurerm_storage_account" "brokerstorage" {
  name                     = var.StorageAccountName 
  resource_group_name      = azurerm_resource_group.brokergroup.name
  location                 = azurerm_resource_group.brokergroup.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}
resource "azurerm_application_insights" "application_insights" {
  name                = "loan-broker-application-insights"
  location            = azurerm_resource_group.brokergroup.location
  resource_group_name = azurerm_resource_group.brokergroup.name
  application_type    = "web"
}

resource "azurerm_app_service_plan" "ASP_bank_1" {
  name                = "${var.Bank1Name}-ASP"
  location            = azurerm_resource_group.brokergroup.location
  resource_group_name = azurerm_resource_group.brokergroup.name
  kind                = "FunctionApp"
  reserved            = true

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_app_service_plan" "ASP_bank_2" {
  name                = "${var.Bank2Name}-ASP"
  location            = azurerm_resource_group.brokergroup.location
  resource_group_name = azurerm_resource_group.brokergroup.name
  kind                = "FunctionApp"
  reserved            = true

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_app_service_plan" "ASP_bank_3" {
  name                = "${var.Bank3Name}-ASP"
  location            = azurerm_resource_group.brokergroup.location
  resource_group_name = azurerm_resource_group.brokergroup.name
  kind                = "FunctionApp"
  reserved            = true

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_app_service_plan" "ASP_broker" {
  name                = "${var.BrokerName}-ASP"
  location            = azurerm_resource_group.brokergroup.location
  resource_group_name = azurerm_resource_group.brokergroup.name
  kind                = "FunctionApp"
  reserved            = true

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_app_service_plan" "ASP_credit" {
  name                = "${var.CreditBureauName}-ASP"
  location            = azurerm_resource_group.brokergroup.location
  resource_group_name = azurerm_resource_group.brokergroup.name
  kind                = "FunctionApp"
  reserved            = true

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_function_app" "bank_1" {
  name                       = var.Bank1Name
  location                   = azurerm_resource_group.brokergroup.location
  resource_group_name        = azurerm_resource_group.brokergroup.name
  app_service_plan_id        = azurerm_app_service_plan.ASP_bank_1.id
  storage_account_name       = azurerm_storage_account.brokerstorage.name
  storage_account_access_key = azurerm_storage_account.brokerstorage.primary_access_key
  os_type                    = "linux"
  version                    = "~3"
  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE" = "",
    "FUNCTIONS_WORKER_RUNTIME" = "dotnet",
    "FUNCTIONS_EXTENSION_VERSION" = "~3"
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.application_insights.instrumentation_key,
    "APPINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.application_insights.connection_string,
  }
}

resource "azurerm_function_app" "bank_2" {
  name                       = var.Bank2Name
  location                   = azurerm_resource_group.brokergroup.location
  resource_group_name        = azurerm_resource_group.brokergroup.name
  app_service_plan_id        = azurerm_app_service_plan.ASP_bank_2.id
  storage_account_name       = azurerm_storage_account.brokerstorage.name
  storage_account_access_key = azurerm_storage_account.brokerstorage.primary_access_key
  os_type                    = "linux"
  version                    = "~3"
  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE" = "",
    "FUNCTIONS_WORKER_RUNTIME" = "dotnet",
    "FUNCTIONS_EXTENSION_VERSION" = "~3"
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.application_insights.instrumentation_key,
    "APPINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.application_insights.connection_string,
  }
}

resource "azurerm_function_app" "bank_3" {
  name                       = var.Bank3Name
  location                   = azurerm_resource_group.brokergroup.location
  resource_group_name        = azurerm_resource_group.brokergroup.name
  app_service_plan_id        = azurerm_app_service_plan.ASP_bank_3.id
  storage_account_name       = azurerm_storage_account.brokerstorage.name
  storage_account_access_key = azurerm_storage_account.brokerstorage.primary_access_key
  os_type                    = "linux"
  version                    = "~3"
  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE" = "",
    "FUNCTIONS_WORKER_RUNTIME" = "dotnet",
    "FUNCTIONS_EXTENSION_VERSION" = "~3"
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.application_insights.instrumentation_key,
    "APPINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.application_insights.connection_string,
  }
}

resource "azurerm_function_app" "broker" {
  name                       = var.BrokerName
  location                   = azurerm_resource_group.brokergroup.location
  resource_group_name        = azurerm_resource_group.brokergroup.name
  app_service_plan_id        = azurerm_app_service_plan.ASP_broker.id
  storage_account_name       = azurerm_storage_account.brokerstorage.name
  storage_account_access_key = azurerm_storage_account.brokerstorage.primary_access_key
  os_type                    = "linux"
  version                    = "~3"
  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE" = "",
    "FUNCTIONS_WORKER_RUNTIME" = "dotnet",
    "FUNCTIONS_EXTENSION_VERSION" = "~3"
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.application_insights.instrumentation_key,
    "APPINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.application_insights.connection_string,
  }
}

resource "azurerm_function_app" "broker" {
  name                       = var.CreditBureauName
  location                   = azurerm_resource_group.brokergroup.location
  resource_group_name        = azurerm_resource_group.brokergroup.name
  app_service_plan_id        = azurerm_app_service_plan.ASP_credit.id
  storage_account_name       = azurerm_storage_account.brokerstorage.name
  storage_account_access_key = azurerm_storage_account.brokerstorage.primary_access_key
  os_type                    = "linux"
  version                    = "~3"
  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE" = "",
    "FUNCTIONS_WORKER_RUNTIME" = "dotnet",
    "FUNCTIONS_EXTENSION_VERSION" = "~3"
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.application_insights.instrumentation_key,
    "APPINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.application_insights.connection_string,
  }
}

resource "azurerm_servicebus_namespace" "servicebus_ns" {
  name                = var.ServiceBusNs
  location            = azurerm_resource_group.brokergroup.location
  resource_group_name = azurerm_resource_group.brokergroup.name
  sku                 = "Standard"

  tags = {
    source = "terraform"
  }
}

resource "azurerm_servicebus_topic" "request_topic" {
  name                = "LoanQuoteRequest"
  resource_group_name = azurerm_resource_group.brokergroup.name
  namespace_name      = azurerm_servicebus_namespace.servicebus_ns.name
}

resource "azurerm_servicebus_queue" "quotation_queue" {
  name                = "LoanQuotation"
  resource_group_name = azurerm_resource_group.brokergroup.name
  namespace_name      = azurerm_servicebus_namespace.servicebus_ns.name
}

resource "azurerm_servicebus_namespace_authorization_rule" "service-bus-auth-rule" {
  name                = "LoanBrokerReadWriteRule"
  namespace_name      = azurerm_servicebus_namespace.servicebus_ns.name
  resource_group_name = azurerm_resource_group.brokergroup.name

  listen = true
  send   = true
}

resource "azurerm_servicebus_subscription" "bank1_subscription" {
  name                = var.Bank1Name
  resource_group_name = azurerm_resource_group.brokergroup.name
  namespace_name      = azurerm_servicebus_namespace.servicebus_ns.name
  topic_name          = azurerm_servicebus_topic.request_topic.name
  max_delivery_count  = 1
}

resource "azurerm_servicebus_subscription" "bank2_subscription" {
  name                = var.Bank2Name
  resource_group_name = azurerm_resource_group.brokergroup.name
  namespace_name      = azurerm_servicebus_namespace.servicebus_ns.name
  topic_name          = azurerm_servicebus_topic.request_topic.name
  max_delivery_count  = 1
}

resource "azurerm_servicebus_subscription" "bank3_subscription" {
  name                = var.Bank3Name
  resource_group_name = azurerm_resource_group.brokergroup.name
  namespace_name      = azurerm_servicebus_namespace.servicebus_ns.name
  topic_name          = azurerm_servicebus_topic.request_topic.name
  max_delivery_count  = 1
}