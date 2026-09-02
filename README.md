RG: auction-rg
CR: auctioncr

API_CONTAINER_APP_NAME: auction-api
FUNCTION_APP_NAME: auction-function

az deployment group what-if `
--resource-group auction-rg `
--template-file main.bicep `
--parameters dev.bicepparam


## Create Item HSD
<img width="1533" height="508" alt="image" src="https://github.com/user-attachments/assets/aa803fb9-f63d-4435-9605-3cc247f25e3c" />
