#!/usr/bin/env bash
# Sets up passwordless GitHub Actions -> Azure auth (OIDC federated credential).
set -euo pipefail

# Prevents Git Bash (MSYS) from mangling "/subscriptions/..." scope arguments into Windows paths.
export MSYS_NO_PATHCONV=1

APP_NAME="auction-tracker-github-actions"
GITHUB_OWNER="vpriadilia"
GITHUB_REPO="auction"
GITHUB_BRANCH="main"

# GitHub issues immutable OIDC subject claims (repo:owner@id/repo@id) for repos created/renamed
# after 2026-07-15, to prevent subject reuse if a repo is later renamed or transferred.
# These IDs are permanent for this repo - grab them again only if the repo is ever recreated.
GITHUB_OWNER_ID="137265697"
GITHUB_REPO_ID="1353653679"

RESOURCE_GROUP="auction-rg"
ACR_NAME="auctioncr"
API_CONTAINER_APP_NAME="auction-api"
FUNCTION_APP_NAME="auction-function"

SUBSCRIPTION_ID=$(az account show --query id -o tsv)
TENANT_ID=$(az account show --query tenantId -o tsv)

APP_ID=$(az ad app list --display-name "$APP_NAME" --query "[0].appId" -o tsv)
if [ -z "$APP_ID" ]; then
  APP_ID=$(az ad app create --display-name "$APP_NAME" --query appId -o tsv)
  echo "Created app registration: $APP_ID"
else
  echo "Reusing existing app registration: $APP_ID"
fi

az ad sp create --id "$APP_ID" >/dev/null 2>&1 || echo "Service principal already exists"

FEDERATED_CRED_NAME="github-actions-${GITHUB_BRANCH}"
FEDERATED_CRED_PARAMS="{
    \"name\": \"${FEDERATED_CRED_NAME}\",
    \"issuer\": \"https://token.actions.githubusercontent.com\",
    \"subject\": \"repo:${GITHUB_OWNER}@${GITHUB_OWNER_ID}/${GITHUB_REPO}@${GITHUB_REPO_ID}:ref:refs/heads/${GITHUB_BRANCH}\",
    \"audiences\": [\"api://AzureADTokenExchange\"]
  }"

if az ad app federated-credential show --id "$APP_ID" --federated-credential-id "$FEDERATED_CRED_NAME" -o none 2>/dev/null; then
  az ad app federated-credential update \
    --id "$APP_ID" \
    --federated-credential-id "$FEDERATED_CRED_NAME" \
    --parameters "$FEDERATED_CRED_PARAMS"
  echo "Updated existing federated credential to immutable subject format"
else
  az ad app federated-credential create \
    --id "$APP_ID" \
    --parameters "$FEDERATED_CRED_PARAMS"
  echo "Created federated credential"
fi

# 4. Least-privilege role assignments, scoped to the specific resources (not the whole subscription)
az role assignment create \
  --assignee "$APP_ID" \
  --role "AcrPush" \
  --scope "/subscriptions/${SUBSCRIPTION_ID}/resourceGroups/${RESOURCE_GROUP}/providers/Microsoft.ContainerRegistry/registries/${ACR_NAME}"

az role assignment create \
  --assignee "$APP_ID" \
  --role "Container Apps Contributor" \
  --scope "/subscriptions/${SUBSCRIPTION_ID}/resourceGroups/${RESOURCE_GROUP}/providers/Microsoft.App/containerApps/${API_CONTAINER_APP_NAME}"

az role assignment create \
  --assignee "$APP_ID" \
  --role "Container Apps Contributor" \
  --scope "/subscriptions/${SUBSCRIPTION_ID}/resourceGroups/${RESOURCE_GROUP}/providers/Microsoft.App/containerApps/${FUNCTION_APP_NAME}"

echo ""
echo "Add these as GitHub repo secrets (Settings -> Secrets and variables -> Actions):"
echo "AZURE_CLIENT_ID=${APP_ID}"
echo "AZURE_TENANT_ID=${TENANT_ID}"
echo "AZURE_SUBSCRIPTION_ID=${SUBSCRIPTION_ID}"
