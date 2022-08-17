![Build Status](https://github.com/TMExperimentals/IdentityApi/actions/workflows/dotnet.yml/badge.svg)

# IdentityApi

Identity api is dedicated identity management system microservice for TM Solution. It has features of managing user accounts as well as managing authentication and authorization.

Environment variables required by service

| Variable | Defaule value |
| :------- | :------------ |
| ASPNETCORE_URLS | http://[*]:8080 |
| ConnectionStrings__IdentityStoreConnectionString | Server=<SERVER_NAME>;Initial Catalog=<DATABASE_NAME>;User ID=<DATABASE_USER>;Password=<DATABASE_PASSWORD>;TrustServerCertificate=False;Connection Timeout=30; |
| JwtSecretKey__PRIVATE_KEY | <PRIVATE_KEY> |
| JwtSecretKey__PUBLIC_KEY | <PUBLIC_KEY> |
| JwtSecretKey__ISSUER | <ISSUER> |
| JwtSecretKey__AUDIENCE | <AUDIENCE> |
| JwtSecretKey__TTL | <JWT_TTL>
