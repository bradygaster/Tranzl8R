param (
    $resourceBaseName="tranzl8r$( Get-Random -Maximum 1000)",
    $location='northcentralus'
)

dotnet build

Write-Host 'Building Web UI' -ForegroundColor Cyan
dotnet publish Tranzl8R.Web\Tranzl8R.Web.csproj

Write-Host 'Building Dashboard' -ForegroundColor Cyan
dotnet publish Tranzl8R.OrleansDashboard\Tranzl8R.OrleansDashboard.csproj

Write-Host 'Creating resource group ' -ForegroundColor Cyan
az group create -l $location -n $resourceBaseName

Write-Host 'Creating resources ' -ForegroundColor Cyan
az deployment group create --resource-group $resourceBaseName --template-file resources.bicep --parameters resourceBaseName=$resourceBaseName

Write-Host 'Deploying code ' -ForegroundColor Cyan
az webapp deploy -n "$($resourceBaseName)-ui" -g $resourceBaseName --src-path ui.zip
az webapp deploy -n "$($resourceBaseName)-dashboard" -g $resourceBaseName --src-path dashboard.zip

Write-Host 'Browsing site ' -ForegroundColor Cyan
az webapp browse -n "$($resourceBaseName)-ui" -g $resourceBaseName

Write-Host 'Browsing dashboard ' -ForegroundColor Cyan
az webapp browse -n "$($resourceBaseName)-dashboard" -g $resourceBaseName
