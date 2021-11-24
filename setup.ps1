param (
    $resourceBaseName="tranzl8r$( Get-Random -Maximum 1000)",
    $location='westus'
)

dotnet build

Write-Host 'Building Web UI' -ForegroundColor Cyan
dotnet publish Tranzl8R.Web\Tranzl8R.Web.csproj

Write-Host 'Building Dashboard' -ForegroundColor Cyan
dotnet publish Tranzl8R.OrleansDashboard\Tranzl8R.OrleansDashboard.csproj

Write-Host 'Creating resource group ' -ForegroundColor Cyan
az group create -l westus -n $resourceBaseName

Write-Host 'Creating container registry ' -ForegroundColor Cyan
az acr create -n "$($resourceBaseName)acr" -g $resourceBaseName --admin-enabled true --sku Basic
$registryPassword=$(az acr credential show -n bradygtests -g bradyg-appservice-tests --query "passwords[0].value")
$registryUsername=$(az acr credential show -n bradygtests -g bradyg-appservice-tests --query "username")

Write-Host 'Creating resources ' -ForegroundColor Cyan
az deployment group create --resource-group $resourceBaseName --template-file resources.bicep --parameters resourceBaseName=$resourceBaseName --parameters containerRegistryUsername=$registryUsername --parameters containerRegistryPassword=$registryPassword

# Write-Host 'Deploying code ' -ForegroundColor Cyan
# az webapp deploy -n "$($resourceBaseName)-ui" -g $resourceBaseName --src-path ui.zip
# az webapp deploy -n "$($resourceBaseName)-dashboard" -g $resourceBaseName --src-path dashboard.zip

# Write-Host 'Browsing site ' -ForegroundColor Cyan
# az webapp browse -n "$($resourceBaseName)-ui" -g $resourceBaseName

# Write-Host 'Browsing dashboard ' -ForegroundColor Cyan
# az webapp browse -n "$($resourceBaseName)-dashboard" -g $resourceBaseName


