<img width="1267" height="706" alt="image" src="https://github.com/user-attachments/assets/3d381911-be23-4af2-9e13-fd713889c163" />

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

### HSD
<img width="1755" height="887" alt="image" src="https://github.com/user-attachments/assets/c380e6b5-b83a-44b7-a9a6-13a35cda5751" />

## UI example
<img width="1213" height="893" alt="image" src="https://github.com/user-attachments/assets/a2b15431-7370-4a2d-8174-f8087847c398" />
