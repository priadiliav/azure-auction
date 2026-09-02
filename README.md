RG: auction-rg
CR: auctioncr

API_CONTAINER_APP_NAME: auction-api
FUNCTION_APP_NAME: auction-function

az deployment group what-if `
--resource-group auction-rg `
--template-file main.bicep `
--parameters dev.bicepparam