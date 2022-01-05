param(    
    [Parameter(Mandatory = $true)]
    [string]$modelId,
    [Parameter]
    [string]$modelDescription,
    [Parameter(Mandatory = $true)]
    [string]$frmRecognizerSourceEndpoint,
    [Parameter(Mandatory = $true)]
    [string]$frmRecognizerSourceKey,
    [Parameter(Mandatory = $true)]
    [string]$frmRecognizerDestinationEndpoint,
    [Parameter(Mandatory = $true)]
    [string]$frmRecognizerDestinationKey    
)

$copyUrl = "https://$frmRecognizerDestinationEndpoint/formrecognizer/documentModels:authorizeCopy?api-version=2021-09-30-preview"
# $deleteUrl = "https://$frmRecognizerDestinationEndpoint/formrecognizer/documentModels/$modelId?api-version=2021-09-30-preview"

# $header = @{
#     "Ocp-Apim-Subscription-Key"="$frmRecognizerDestinationKey"
# } 

# $response = Invoke-WebRequest -Uri $deleteUrl -Method 'Delete' -Headers $header

Write-Output $response

$header = @{
  "Ocp-Apim-Subscription-Key"="$frmRecognizerDestinationKey"
  "Content-Type"="application/json"
} 

$body= @{
    "modelId"="$modelId"
    "description"="$modelDescription"
} | ConvertTo-Json

$response = Invoke-WebRequest -Uri $copyUrl -Method 'Post' -Body $body -Headers $header

if ($response.StatusCode -eq 200) {
    $content = $response.Content | ConvertFrom-Json
    Write-Output $content.accessToken
    
} else {
    
}