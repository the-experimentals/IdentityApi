

<div align="center">
    <h1>Identity API</h1>
    <a href="https://github.com/itsbibeksaini/IdentityApi/actions/workflows/dotnet.yml">
        <img src="https://github.com/itsbibeksaini/IdentityApi/actions/workflows/dotnet.yml/badge.svg">
    </a>
</div>




Identity api is dedicated identity management system microservice for TM Solution. It has features of managing user accounts as well as managing authentication and authorization.

## Installation

1. Local system
    clone the repository and run:
    1. `dotnet restore` to restore all dependencies. 
    2. `dotnet build` to build solution.
    3. `donet run` to run project.
    
2. Install on local k8s cluster through helm chart available in repository.
    1. Create docker image for service/project on local system:
        ```
        docker build --pull --rm -f "Dockerfile" -t identityapi:0.0.1 "."
        ```
    2. Install helm charts
        ```
        helm upgrade identityapi --install ./identityapi-charts
        ```

Environment variables required by service

| Variable | Defaule value |
| :------- | :------------ |
| ASPNETCORE_URLS | http://[*]:8080 |
| ConnectionStrings__IdentityStoreConnectionString | Server=<SERVER_NAME>;Initial Catalog=<DATABASE_NAME>;User ID=<DATABASE_USER>;Password=<DATABASE_PASSWORD>;TrustServerCertificate=False;Connection Timeout=30; |
| JwtSecretKey__PRIVATE_KEY | <PRIVATE_KEY> |
| JwtSecretKey__PUBLIC_KEY | <PUBLIC_KEY> |
| JwtSecretKey__ISSUER | < ISSUER > |
| JwtSecretKey__AUDIENCE | < AUDIENCE > |
| JwtSecretKey__TTL | <JWT_TTL>

## Consuming service
  Follow the swagger api documentation to explore various service endpoints at: (doc)[http://localhost:8080/swagger]
  > Note: launching project through `Visual Studio` IDE opens service's swagger documentation by default.
