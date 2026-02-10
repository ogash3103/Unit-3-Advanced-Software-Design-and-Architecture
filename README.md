# Digital Procurement Platform for Agriculture & Food Production

## 📌 Overview

This project implements a **Digital Procurement Platform for Agriculture & Food Production** designed as a **sector-wide digital infrastructure**. The platform enables transparent procurement, supplier participation, competitive bidding, and contract lifecycle support using modern software architecture and DevOps practices.

The solution was developed as part of **Unit 3 – Advanced Software Design and Architecture (Level 6)** and demonstrates:
- Clean Architecture with layered and modular design
- Domain-Driven Design (DDD) principles
- Cloud-native deployment using Kubernetes
- CI/CD automation using GitHub Actions and ArgoCD (GitOps)
- Scalable and secure RESTful API design

---

## 🎯 Platform Objectives

The platform addresses common challenges in agricultural procurement:
- Limited access to real-time market opportunities
- Fragmented procurement processes across regions
- Manual supplier qualification and verification
- Lack of transparency in bidding and contract awards
- Inefficient communication between buyers and suppliers

### Key Goals:
- Enable fair and digital participation for suppliers across regions
- Support scalable procurement opportunities with automated workflows
- Ensure reliable, auditable, and transparent transactions
- Allow future regulatory compliance and system evolution
- Provide real-time visibility into procurement lifecycle

---

## 🏗️ Architecture Overview

The system follows **Clean Architecture** principles with clear separation of concerns across four layers:

```
AgriProcurementPlatform/
│
├── Procurement.Api/              → Presentation Layer
│   ├── Controllers/              → REST API endpoints
│   ├── Middleware/               → Exception handling
│   ├── Background/               → Outbox processor
│   └── Program.cs                → Application entry point
│
├── Procurement.Application/      → Application Layer
│   ├── Services/                 → Business orchestration
│   ├── Contracts/                → Commands and DTOs
│   └── Abstractions/             → Interface definitions
│
├── Procurement.Domain/           → Domain Layer
│   ├── Opportunities/            → Opportunity aggregate
│   ├── Suppliers/                → Supplier aggregate
│   ├── Bids/                     → Bid aggregate
│   └── Common/                   → Domain events, base entities
│
├── Procurement.Infrastructure/   → Infrastructure Layer
│   ├── Persistence/              → EF Core DbContext
│   ├── Outbox/                   → Outbox pattern implementation
│   └── Migrations/               → Database migrations
│
├── deploy/
│   ├── charts/                   → Helm charts for Kubernetes
│   └── argocd/                   → ArgoCD application manifest
│
└── .github/workflows/            → CI/CD pipeline
```

### Architectural Principles

1. **Dependency Inversion**: Domain layer has no dependencies; all layers depend inward
2. **Separation of Concerns**: Each layer has distinct responsibilities
3. **Domain-Driven Design**: Rich domain models with business logic encapsulation
4. **Loose Coupling**: Interfaces define contracts between layers
5. **Extensibility**: Easy to add new features without modifying existing code

### Key Design Patterns

- **Repository Pattern**: Abstraction over data access
- **Outbox Pattern**: Reliable event publishing with transactional guarantees
- **Domain Events**: Decoupled communication between aggregates
- **CQRS (Command Query Responsibility Segregation)**: Separate read and write operations
- **Dependency Injection**: Loose coupling and testability

---

## 🔌 API Capabilities

The platform exposes a RESTful API with comprehensive procurement functionality:

### Opportunities Management
- `POST /api/opportunities` - Create new procurement opportunity
- `GET /api/opportunities` - List all opportunities
- `GET /api/opportunities/{id}` - Get opportunity details
- `POST /api/opportunities/{id}/close` - Close opportunity (end bidding)

### Supplier Management
- `POST /api/suppliers` - Register new supplier
- `GET /api/suppliers` - List all suppliers
- `GET /api/suppliers/{id}` - Get supplier details
- `POST /api/suppliers/{id}/qualify` - Qualify supplier for bidding

### Bid Management
- `POST /api/bids` - Submit bid for opportunity
- `GET /api/bids/opportunity/{opportunityId}` - Get all bids for opportunity

### Health & Monitoring
- `GET /health` - Health check endpoint for Kubernetes probes

📄 **Interactive API documentation** is available via **Swagger UI** at `/swagger` endpoint.

---

## 🗄️ Data Management

### Database Architecture
- **PostgreSQL 16** as the primary relational database
- **Entity Framework Core 8** for ORM and migrations
- **Outbox Pattern** for reliable event handling

### Domain Models

**Opportunity**
- Represents procurement opportunities with product category, quantity, deadline
- Supports lifecycle states (Open, Closed)
- Emits domain events (OpportunityCreated, OpportunityClosed)

**Supplier**
- Represents registered suppliers with legal name and region
- Tracks qualification status
- Emits domain events (SupplierRegistered, SupplierQualified)

**Bid**
- Represents supplier bids with unit price and submission time
- Links to opportunity and supplier
- Emits domain events (BidSubmitted)

### Outbox Pattern Implementation

The Outbox pattern ensures reliable event publishing:
1. Domain events are saved to `OutboxMessages` table in the same transaction
2. Background processor (`OutboxProcessor`) polls for unprocessed messages
3. Events are published to external systems (future: message broker)
4. Processed messages are marked as completed

This approach guarantees:
- **Transactional consistency** between domain changes and events
- **At-least-once delivery** semantics
- **Auditability** of all domain events
- **Future-ready integration** with messaging systems (RabbitMQ, Kafka)

---

## ⚙️ DevOps & Deployment

### CI Pipeline – GitHub Actions

The CI/CD pipeline (`.github/workflows/ci-cd.yml`) automatically:

1. **Checkout code** from the repository
2. **Setup .NET 8 SDK** environment
3. **Restore dependencies** for the solution
4. **Build** the solution in Release mode
5. **Login to Docker Hub** using secrets
6. **Build Docker image** with tag `ogabek0331/procurement-api:latest`
7. **Push image** to Docker Hub registry

**Trigger**: Automatically runs on every push to `main` branch

**Benefits**:
- Ensures code quality before deployment
- Automates container image creation
- Provides consistent build environment
- Reduces manual deployment errors

### CD Pipeline – ArgoCD (GitOps)

ArgoCD continuously monitors the Git repository and automatically syncs changes to Kubernetes:

**Configuration** (`deploy/argocd/procurement-app.yaml`):
- **Source**: GitHub repository with Helm charts
- **Destination**: Kubernetes cluster, `procurement` namespace
- **Sync Policy**: Automated with self-healing and pruning
- **Auto-create namespace**: Enabled

**GitOps Benefits**:
- **Declarative infrastructure**: All configuration in Git
- **Automated deployments**: No manual kubectl commands
- **Self-healing**: Automatically corrects drift from desired state
- **Rollback support**: Easy revert to previous Git commit
- **Audit trail**: Git history tracks all changes

---

## ☸️ Kubernetes Deployment

### Helm Chart Structure

The platform uses Helm for templated Kubernetes manifests:

**Chart Components** (`deploy/charts/procurement-api/`):
- `Chart.yaml` - Chart metadata and version
- `values.yaml` - Configuration values
- `templates/deployment.yaml` - API deployment
- `templates/postgres.yaml` - PostgreSQL StatefulSet
- `templates/service.yaml` - Kubernetes services

### Deployment Configuration

**API Deployment**:
- **Image**: `ogabek0331/procurement-api:latest`
- **Replicas**: 1 (configurable for scaling)
- **Port**: 8080 (container), 80 (service)
- **Environment**: Production with PostgreSQL connection string
- **Health checks**: Liveness and readiness probes on `/health`

**PostgreSQL Deployment**:
- **Image**: `postgres:16`
- **Storage**: Persistent volume for data durability
- **Credentials**: Configured via environment variables
- **Database**: `procurementdb`

**Networking**:
- **Service Type**: ClusterIP (internal access)
- **Namespace**: `procurement` (isolated environment)
- **Port Forwarding**: For local access during development

### Accessing the Application

```bash
# Port forward to local machine
kubectl port-forward svc/procurement-api -n procurement 8081:80

# Access Swagger UI
open http://localhost:8081/swagger
```

---

## 🔐 Security & Reliability Considerations

### Current Implementation
- **Layer isolation**: Clear boundaries prevent unauthorized access
- **Environment-based configuration**: Secrets managed via Kubernetes
- **Health checks**: Kubernetes monitors application health
- **Exception handling**: Global middleware catches and logs errors
- **Database transactions**: ACID guarantees for data consistency

### Future Enhancements
- **Authentication & Authorization**: JWT-based API security
- **Role-based access control (RBAC)**: Supplier vs. buyer permissions
- **Audit logging**: Track all user actions and changes
- **Rate limiting**: Prevent API abuse
- **TLS/SSL**: Encrypted communication
- **Secret management**: Integration with Vault or AWS Secrets Manager
- **Network policies**: Kubernetes-level traffic control

---

## 🚀 How to Run

### Prerequisites

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)
- **Kubernetes** (Minikube or Docker Desktop)
- **Helm 3** - [Install](https://helm.sh/docs/intro/install/)
- **kubectl** - [Install](https://kubernetes.io/docs/tasks/tools/)
- **ArgoCD** (optional for GitOps) - [Install](https://argo-cd.readthedocs.io/en/stable/getting_started/)

### Local Development (Without Kubernetes)

```bash
# 1. Clone the repository
git clone https://github.com/ogash3103/Unit-3-Advanced-Software-Design-and-Architecture.git
cd Unit-3-Advanced-Software-Design-and-Architecture/AgriProcurementPlatform

# 2. Start PostgreSQL with Docker Compose
docker-compose up -d

# 3. Apply database migrations
cd Procurement.Api
dotnet ef database update --project ../Procurement.Infrastructure

# 4. Run the API
dotnet run

# 5. Access Swagger UI
open http://localhost:5000/swagger
```

### Kubernetes Deployment (Local)

```bash
# 1. Start Minikube
minikube start

# 2. Install with Helm
helm install procurement-api ./deploy/charts/procurement-api -n procurement --create-namespace

# 3. Wait for pods to be ready
kubectl wait --for=condition=ready pod -l app=procurement-api -n procurement --timeout=300s

# 4. Port forward to access
kubectl port-forward svc/procurement-api -n procurement 8081:80

# 5. Access Swagger UI
open http://localhost:8081/swagger
```

### Kubernetes Deployment (ArgoCD)

```bash
# 1. Install ArgoCD
kubectl create namespace argocd
kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml

# 2. Access ArgoCD UI
kubectl port-forward svc/argocd-server -n argocd 8080:443

# 3. Get admin password
kubectl -n argocd get secret argocd-initial-admin-secret -o jsonpath="{.data.password}" | base64 -d

# 4. Deploy application
kubectl apply -f deploy/argocd/procurement-app.yaml

# 5. Monitor deployment in ArgoCD UI
open https://localhost:8080
```

---

## 📊 Testing the API

### Sample API Requests

**1. Register a Supplier**
```bash
curl -X POST http://localhost:8081/api/suppliers \
  -H "Content-Type: application/json" \
  -d '{
    "legalName": "Green Valley Farms Ltd",
    "regionCode": "UZ-TAS"
  }'
```

**2. Qualify the Supplier**
```bash
curl -X POST http://localhost:8081/api/suppliers/{supplierId}/qualify
```

**3. Create Procurement Opportunity**
```bash
curl -X POST http://localhost:8081/api/opportunities \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Fresh Tomatoes - 5000kg",
    "productCategory": "Vegetables",
    "quantity": 5000,
    "deadlineUtc": "2026-03-01T00:00:00Z",
    "regionCode": "UZ-TAS"
  }'
```

**4. Submit a Bid**
```bash
curl -X POST http://localhost:8081/api/bids \
  -H "Content-Type: application/json" \
  -d '{
    "opportunityId": "{opportunityId}",
    "supplierId": "{supplierId}",
    "unitPrice": 2.50
  }'
```

**5. Get All Bids for Opportunity**
```bash
curl http://localhost:8081/api/bids/opportunity/{opportunityId}
```

---

## 📈 Scalability & Future Improvements

### Scalability Features

The platform is designed for horizontal scaling:
- **Stateless API**: Can run multiple replicas behind load balancer
- **Database connection pooling**: Efficient resource utilization
- **Kubernetes HPA**: Auto-scaling based on CPU/memory
- **Outbox pattern**: Supports eventual consistency for distributed systems

### Planned Enhancements

**Technical**:
- Message broker integration (RabbitMQ/Kafka) for event-driven architecture
- Redis caching for read-heavy operations
- Elasticsearch for advanced search and analytics
- GraphQL API for flexible client queries
- gRPC for inter-service communication

**Business**:
- Contract management and digital signatures
- Payment processing integration
- Supplier rating and review system
- Real-time notifications (WebSockets/SignalR)
- Multi-language support
- Mobile application
- Analytics dashboard for procurement insights

**Operations**:
- Distributed tracing (OpenTelemetry)
- Centralized logging (ELK stack)
- Metrics and monitoring (Prometheus/Grafana)
- Chaos engineering for resilience testing

---

## 📚 Academic Context

This project demonstrates learning outcomes for **Unit 3 – Advanced Software Design and Architecture (Level 6)**:

### Learning Outcomes Addressed

**LO1: Architectural Styles**
- Clean Architecture with layered design
- Domain-Driven Design principles
- Event-driven architecture (Outbox pattern)

**LO2: Quality Attributes**
- **Scalability**: Horizontal scaling with Kubernetes
- **Maintainability**: Clear separation of concerns
- **Reliability**: Outbox pattern for consistency
- **Security**: Layer isolation and future auth support
- **Deployability**: Automated CI/CD pipeline

**LO3: Cloud-Native Design**
- Containerization with Docker
- Kubernetes orchestration
- Helm for configuration management
- GitOps with ArgoCD

**LO4: DevOps Practices**
- CI pipeline with GitHub Actions
- CD pipeline with ArgoCD
- Infrastructure as Code (Helm charts)
- Automated testing and deployment

### Sector-Wide Platform Design

This is a **sector-wide digital infrastructure** rather than a single-organization system:
- Serves multiple buyers and suppliers across regions
- Supports standardized procurement processes
- Enables fair competition and transparency
- Allows regulatory compliance and evolution
- Provides shared infrastructure for cost efficiency

---

## 🛠️ Technology Stack

**Backend**:
- .NET 8 (C#)
- ASP.NET Core Web API
- Entity Framework Core 8
- PostgreSQL 16

**DevOps**:
- Docker & Docker Compose
- Kubernetes
- Helm 3
- ArgoCD
- GitHub Actions

**Tools**:
- Swagger/OpenAPI for API documentation
- Npgsql for PostgreSQL connectivity
- Background services for async processing

---

## 📁 Project Structure Details

```
AgriProcurementPlatform/
├── Procurement.Api/
│   ├── Controllers/              # REST API endpoints
│   │   ├── OpportunitiesController.cs
│   │   ├── SuppliersController.cs
│   │   └── BidsController.cs
│   ├── Services/                 # Query services
│   ├── Background/               # Outbox processor
│   ├── Middleware/               # Exception handling
│   ├── Contracts/                # Response DTOs
│   └── Program.cs                # DI configuration
│
├── Procurement.Application/
│   ├── Services/                 # Business logic
│   │   └── ProcurementService.cs
│   ├── Contracts/                # Commands
│   └── Abstractions/             # Interfaces
│
├── Procurement.Domain/
│   ├── Opportunities/            # Opportunity aggregate
│   ├── Suppliers/                # Supplier aggregate
│   ├── Bids/                     # Bid aggregate
│   └── Common/                   # Base classes, events
│
└── Procurement.Infrastructure/
    ├── Persistence/              # DbContext
    ├── Outbox/                   # Outbox implementation
    └── Migrations/               # EF Core migrations
```



## 👤 Author

**Ogabek Faxriddinov**  
Level 6 – Advanced Software Design & Architecture  
Digital Technologies  
PDP University in Tashkent





