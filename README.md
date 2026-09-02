RG: auction-rg
CR: auctioncr

API_CONTAINER_APP_NAME: auction-api
FUNCTION_APP_NAME: auction-function

az deployment group what-if `
--resource-group auction-rg `
--template-file main.bicep `
--parameters dev.bicepparam


## Create Item HSD
<img width="1573" height="721" alt="image" src="https://github.com/user-attachments/assets/32c2d8ad-b5a9-40f6-94b6-5fd4efca7844" />

