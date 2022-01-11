param(
    [Parameter(Mandatory = $true)]
    [string]$modelId,
    [Parameter(Mandatory = $true)]
    [string]$endpoint
)

try {

    $values = $endpoint.Split("?")
    $endpoint = $values[0]
    $code = $values[1].Replace("code=","")

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