

```md
# Digital Procurement Platform for Agriculture & Food Production

## 📌 Overview
This project implements a **Digital Procurement Platform for Agriculture & Food Production** designed as a **sector-wide digital infrastructure**.  
The platform enables transparent procurement, supplier participation, bidding, and contract lifecycle support using modern software architecture and DevOps practices.

The solution was developed as part of **Unit 3 – Advanced Software Design and Architecture (Level 6)** and demonstrates:
- layered and modular software architecture,
- scalable and secure API design,
- cloud-native deployment using Kubernetes,
- CI/CD automation using GitHub Actions and ArgoCD (GitOps).

---

## 🎯 Platform Objectives
The platform addresses common challenges in agricultural procurement:
- limited access to real-time market opportunities,
- fragmented procurement processes,
- manual supplier qualification,
- lack of transparency in bidding.

### Key goals:
- enable fair and digital participation for suppliers,
- support scalable procurement opportunities,
- ensure reliable and auditable transactions,
- allow future regulatory and system evolution.

---

## 🏗️ Architecture Overview

The system follows a **Clean Architecture / Layered Architecture** approach.

```

AgriProcurementPlatform
│
├── Procurement.Api            → REST API (Controllers, Swagger)
├── Procurement.Application    → Application logic, commands, services
├── Procurement.Domain         → Core domain models and rules
├── Procurement.Infrastructure → Persistence, EF Core, Outbox
│
├── deploy/
│   ├── charts/                → Helm chart for Kubernetes deployment
│   └── argocd/                → ArgoCD Application (GitOps)
│
└── .github/workflows           → CI pipeline (GitHub Actions)

````

### Architectural Principles
- **Separation of concerns** between domain, application, and infrastructure
- **Domain-driven design (DDD) concepts** for procurement logic
- **Loose coupling** between layers
- **Extensibility** for future regulatory and business changes

---

## 🔌 API Capabilities

The platform exposes a RESTful API with the following features:

### Opportunities
- Create procurement opportunities
- Retrieve all or single opportunities
- Close opportunities when bidding ends

### Suppliers
- Register suppliers
- Qualify suppliers for procurement participation
- List registered suppliers

### Bids
- Submit bids for opportunities
- Retrieve bids by opportunity

📄 API documentation is available via **Swagger UI** once deployed.

---

## 🗄️ Data Management
- PostgreSQL is used as the primary relational database
- Entity Framework Core manages persistence
- An **Outbox pattern** is implemented to support reliable event handling and future integrations

This approach ensures:
- transactional consistency,
- future-ready integration with messaging systems,
- auditability of domain events.

---

## ⚙️ DevOps & Deployment

### CI – GitHub Actions
A CI pipeline automatically:
- restores and builds the .NET 8 solution,
- builds a Docker image for the API,
- pushes the image to Docker Hub.

This ensures every commit is validated before deployment.

### CD – ArgoCD (GitOps)
- ArgoCD continuously monitors the Git repository
- Helm charts define Kubernetes resources
- Any change in Git is automatically synced to the cluster

Benefits:
- declarative infrastructure,
- automated deployments,
- self-healing and rollback support.

---

## ☸️ Kubernetes Deployment
The platform is deployed on Kubernetes using:
- **Helm charts** for templated manifests,
- **Deployments** for the API and PostgreSQL,
- **Services** for internal networking,
- **Namespaces** for isolation.

The application health is monitored through Kubernetes and ArgoCD.

---

## 🔐 Security & Reliability Considerations
- Clear separation between application layers
- Controlled access via Kubernetes networking
- Environment-based configuration
- CI/CD pipeline reduces manual deployment errors

The architecture supports future enhancements such as:
- authentication and authorization,
- audit logging,
- policy-based access control.

---

## 🚀 How to Run (Development)

### Prerequisites
- .NET 8 SDK
- Docker
- Kubernetes (Minikube)
- Helm
- ArgoCD

### Local Development
```bash
dotnet restore
dotnet build
dotnet run --project Procurement.Api
````

### Kubernetes Access

```bash
kubectl port-forward svc/procurement-api -n procurement 8081:80
```

Open in browser:

```
http://localhost:8081/swagger
```

---

## 📈 Scalability & Future Improvements

The platform is designed to scale and evolve:

* horizontal scaling via Kubernetes,
* integration with external pricing and logistics systems,
* event-driven extensions using messaging,
* enhanced observability and monitoring.

---

## 📚 Academic Context

This project demonstrates learning outcomes related to:

* advanced software architecture styles,
* quality attribute trade-offs,
* cloud-native application design,
* DevOps and GitOps practices.

It provides a realistic example of a **sector-wide digital platform**, rather than a single-organisation system.

---

## 👤 Author

**Ogabek Faxriddinov**
Level 6 – Advanced Software Design & Architecture
Digital Technologies


