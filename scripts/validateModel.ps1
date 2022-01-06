param(
    [Parameter(Mandatory = $true)]
    [string]$modelId,
    [Parameter(Mandatory = $true)]
    [string]$endpoint,
    [Parameter(Mandatory = $true)]
    [string]$code
)

try {
    $header = @{        
        "Content-Type"="application/json"
        "x-functions-key"="$code"
    } 
    Write-Output $code
    $url = $endpoint + "?modelId=$modelId&environment=0" 
    Write-Output $url

    $response = Invoke-WebRequest -Uri $url -Method 'Get' -Headers $header

    if ($response.StatusCode -ne 200) {
        throw "Error, statusCode: $response.StatusCode"
    }
}
catch {    
    throw $PSItem    
}