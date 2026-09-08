<img width="1073" height="540" alt="image" src="https://github.com/user-attachments/assets/1bc7f1c4-570f-480b-8cf3-fbf21517973d" />

### About this project

A full-stack online auction platform, built as a learning/portfolio project to go deep on a real, event-driven Azure architecture rather than just another CRUD app. It's aimed at:

- Real-time bidding - live price updates, recent-bidder lists, and auction-close notifications pushed to every connected client via SignalR, no polling.
- Event-driven processing - item creation, image upload, and bid placement are decoupled from their downstream effects (status pipeline, notifications, interaction logging) through Service Bus and Event Grid instead of synchronous calls.
- Content-based recommendations - item title/description are embedded via Azure OpenAI, and a user's bid/win history is used to rank their "all items" feed by semantic similarity to what they've shown interest in.
- Concurrency-safe bidding - simultaneous bids on the same item are resolved with an optimistic-concurrency check in the database, not a queue or a lock.
- Secretless auth to Azure - every service-to-service connection (SQL, Storage, Service Bus, SignalR, OpenAI) uses managed identity; no connection strings or keys in config.
- Real user auth - Google Sign-In, with the backend validating ID tokens directly rather than delegating to a separate auth service.

It's a pet project, not a production auction site - the goal was depth on a handful of real distributed-systems problems (bid concurrency, async pipelines, live push, recommendations) rather than breadth of marketplace features.

### Used Services
- Azure Container Registry - stores container images for the API and Function App
- Azure Container Apps Environment - hosts both Container Apps
- Azure Container App (API) - ASP.NET Core Web API: items, bids, auth, /me
- Azure Container App (Function App, Functions-on-Container-Apps) - isolated-worker Azure Functions: blob-SAS issuance, SignalR negotiate, item processing pipeline, blob-created/Event Grid trigger, bid/state/interaction notifiers, auction-closing timer
- Azure SQL Server + Azure SQL Database (serverless, Entra ID-only auth) - stores items, bids, users, interactions
- Azure Storage Account - Function App's own runtime storage (AzureWebJobsStorage)
- Azure Storage Account - item image blobs (public-read container, direct browser upload via SAS)
- Azure Service Bus (queues: items, states, bids, interactions) - decouples writes from async processing/notification
- Azure Event Grid (system topic on the image storage account) - fires blob-created events into the image-processing trigger
- Azure SignalR Service (serverless mode) - pushes live item-status and bid updates to connected clients
- Managed Identity (system-assigned, per Container App) - grants Storage/Service Bus/SignalR/Blob/OpenAI/SQL access without secrets
- Google Sign-In (OAuth/OIDC, external) - user authentication; backend validates Google-issued JWTs directly
- Azure Static Web App - hosts the React/Vite frontend, CI/CD from GitHub
- Azure Application Insights + Log Analytics Workspace - monitoring/logging across both Container Apps

<img width="1055" height="597" alt="image" src="https://github.com/user-attachments/assets/726eae73-02c8-49ef-8229-6de5be3b1c0c" />

### UI example
<img width="1213" height="893" alt="image" src="https://github.com/user-attachments/assets/a2b15431-7370-4a2d-8174-f8087847c398" />
