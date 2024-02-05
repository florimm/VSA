$env:AWS_PROFILE = 'platform_dev'
dapr run `
    --app-id template `
    --app-port 5500 `
    --dapr-http-port 3506 `
    dotnet run
