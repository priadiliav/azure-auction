<img width="1073" height="540" alt="image" src="https://github.com/user-attachments/assets/1bc7f1c4-570f-480b-8cf3-fbf21517973d" />


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
