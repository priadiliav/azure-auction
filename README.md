RG: auction-rg
CR: auctioncr

API_CONTAINER_APP_NAME: auction-api
FUNCTION_APP_NAME: auction-function

az deployment group what-if `
--resource-group auction-rg `
--template-file main.bicep `
--parameters dev.bicepparam


## Create Item HSD
<img width="1666" height="835" alt="image" src="https://github.com/user-attachments/assets/45fe6638-5ea8-491b-8ac2-282150316ea2" />


