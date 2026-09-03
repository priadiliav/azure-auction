RG: auction-rg
CR: auctioncr

API_CONTAINER_APP_NAME: auction-api
FUNCTION_APP_NAME: auction-function

az deployment group what-if `
--resource-group auction-rg `
--template-file main.bicep `
--parameters dev.bicepparam


## Create Item HSD
<img width="1520" height="759" alt="image" src="https://github.com/user-attachments/assets/3b048a30-d58d-42c4-87b9-86bef508804e" />


