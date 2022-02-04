# Loan Broker - Azure Functions
A version of the Loan Broker example from Enterprise Integration Patterns using Azure Durable Functions, Azure Service Bus and Azure Table Storage.

Adapted from Gregor Hohpe's AWS implementation found at https://www.enterpriseintegrationpatterns.com/ramblings/loanbroker_stepfunctions.html

The overall workflow of the broker application is controlled by an Azure Durable Function. The broker is responsible for obtaining a credit score from a separate 'Credit Bureau' service (implemented as a HTTP Triggered Azure Function) and requesting loan quotations from multiple 'Bank' services. 

Loan quotations are requested via a Service Bus Topic which the various bank services (implmented as Service Bus Triggered functions) subscribe to.

Each bank service then provides a quote to the broker via a Service Bus Queue. A queue triggered function in the broker collects quotations, storing the quotations in Azure Table Storage. Once the broker has received the appropriate number of quotes the results are emailed to the requestor using SendGrid.

All the infrastructure can be deployed using Terraform and an Azure DevOps pipeline.
