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
  location = "UK South"
}

resource "azurerm_storage_account" "brokerstorage" {
  name                     = "LoanBrokerStorage"
  resource_group_name      = azurerm_resource_group.brokergroup.name
  location                 = azurerm_resource_group.brokergroup.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}