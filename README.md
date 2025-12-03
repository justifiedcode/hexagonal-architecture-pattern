# MakeTransfer - Hexagonal Architecture Example

A practical example of hexagonal architecture (ports & adapters pattern) implemented in .NET 8 for a money transfer system.

## Overview

This repository demonstrates how to structure a .NET application using hexagonal architecture pattern. The code shows clean separation between business logic and external concerns like databases, external APIs, and notification services.

<img width="1864" height="1545" alt="5 example_product_image" src="https://github.com/user-attachments/assets/421a35fd-e166-43d8-a2bd-298b44afe66b" />

## What You'll Learn

- How to isolate business logic from external dependencies
- Implementing ports and adapters pattern
- Testable code structure

## Project Structure

```
MakeTransfer/
├── Domain/
│   └── MakeTransfer.Core/  # Business logic and domain rules
├── DrivingAdapters/
│   └── MakeTransfer.Api/      # REST API (inbound)
├── DrivenAdapters/
│   ├── MakeTransfer.Adapters.Database/      # Data storage (outbound)
│   ├── MakeTransfer.Adapters.Payment/    # Payment gateway (outbound)
│   └── MakeTransfer.Adapters.Notification/  # Notifications (outbound)
└── Tests/
    └── MakeTransfer.Core.Tests/     # Unit tests
```

## Key Architecture Concepts Demonstrated

### Ports (Interfaces)
- **Input Ports**: `IBankingOperationsInputPort`, `IBankingInquiryInputPort`
- **Output Ports**: `IDatabaseOutputPort`, `IPaymentOutputPort`, `INotificationOutputPort`

### Adapters (Implementations)
- **Primary Adapters**: REST API controllers that drive the application
- **Secondary Adapters**: Database, payment gateway, and notification implementations

### Benefits Shown
- **Testability**: Business logic can be tested without external dependencies
- **Flexibility**: Easy to swap implementations (e.g., different databases)
- **Maintainability**: Clear boundaries between different concerns

## Quick Start

```bash
# Clone the repository
git clone https://github.com/justifiedcode/hexagonal-architecture-pattern.git
cd hexagonal-architecture-pattern

# Build the solution
dotnet build

# Run the API
cd DrivingAdapters/MakeTransfer.Api
dotnet run

# View the API documentation
http://localhost:5183/swagger
```

## Example Usage

### Transfer Money Between Accounts
```bash
curl -X POST "http://localhost:5183/api/transfers" \
  -H "Content-Type: application/json" \
  -d '{
    "fromAccountId": "ACC001",
    "toAccountId": "ACC002",
    "amount": 250.00,
    "currency": "USD",
    "reference": "Payment for services"
  }'
```

### Check Account Balance
```bash
curl -X GET "http://localhost:5183/api/accounts/ACC001/balance"
```

## Sample Data

The application includes demo accounts for testing:

| Account ID | Owner | Currency | Balance | Daily Limit |
|------------|-------|----------|---------|-------------|
| ACC001 | John Doe | USD | $5,000 | $1,000 |
| ACC002 | Jane Smith | USD | $2,500 | $500 |
| ACC003 | Bob Johnson | EUR | €3,000 | €750 |

## Features Implemented

- **Money Transfers**: Transfer funds between accounts with validation
- **Account Management**: View account details and balances  
- **Business Rules**: Sufficient funds, currency matching, daily limits
- **Error Handling**: Proper validation and error responses

## Technology Stack

- **.NET 8** - Framework and runtime
- **ASP.NET Core** - Web API framework
- **xUnit** - Unit testing framework
- **Swagger/OpenAPI** - API documentation

## Running Tests

```bash
dotnet test Tests/MakeTransfer.Core.Tests
```

All business logic is thoroughly tested without requiring external dependencies.

## Code Highlights

### Domain Isolation
The core business logic has zero dependencies on external frameworks:
```csharp
// Pure domain logic
public void Debit(decimal amount, DateOnly today)
{
    if (!HasSufficientBalance(amount))
        throw new InvalidOperationException("Insufficient funds.");
    
    if (!IsWithinDailyLimit(amount, today))
        throw new InvalidOperationException("Daily limit exceeded.");
    
    Balance -= amount;
    DailyDebitedAmount += amount;
}
```

### Use Case Orchestration
Use cases coordinate domain logic and external adapters:
```csharp
public OperationResult<TransferResult> ExecuteTransfer(TransferData transferData)
{
    // Get domain entities through database port
    var fromAccount = _databasePort.GetAccountById(transferData.FromAccountId);
    var toAccount = _databasePort.GetAccountById(transferData.ToAccountId);
    
    // Apply business rules using domain logic
    fromAccount.Debit(transferData.Amount, DateOnly.FromDateTime(DateTime.UtcNow));
    toAccount.Credit(transferData.Amount);
    
    // Persist changes through database port
    _databasePort.ExecuteTransfer(fromAccount, toAccount, transfer);
    
    // Process payment through payment port
    var paymentResult = _paymentPort.ExecuteTransfer(transfer);
    
    // Send notification through notification port
    _notificationPort.NotifyTransfer(transfer, "Transfer completed");
    
   return OperationResult<TransferResult>.SuccessResult(result);
}
```

### Port Definition
Clear boundaries defined by interfaces:
```csharp
public interface IPaymentOutputPort
{
    PaymentResult ExecuteTransfer(Transfer transfer);
}
```

### Adapter Implementation
External concerns handled in adapters:
```csharp
public class PaymentGatewayAdapter : IPaymentOutputPort
{
    public PaymentResult ExecuteTransfer(Transfer transfer)
    {
       // Handle external payment processing
    }
}
```

## Use This Code

Feel free to download, study, and use this code as a reference for implementing hexagonal architecture in your own projects. The structure and patterns shown here can be adapted to different domains and requirements.

## Visual Guide (PDF)
If you prefer a structured explanation, I put together a companion PDF that covers the problem, the solution, the key considerations, when the pattern fits, when it doesn’t, and the system benefits. [Hexagonal Architecture Pattern](https://www.justifiedcode.com/hexagonal-architecture-pattern)

## Learning Resources

- [Hexagonal Architecture by Alistair Cockburn](https://alistair.cockburn.us/hexagonal-architecture/)
- [AWS Hexagonal Architecture](https://docs.aws.amazon.com/prescriptive-guidance/latest/cloud-design-patterns/hexagonal-architecture.html)
