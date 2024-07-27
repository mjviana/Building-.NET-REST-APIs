# Game Store Api

## Starting SQL Server in Docker

```powershell
$sa_password = "[SA PASSWORD HERE]"
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=$sa_password" -p 1433:1433 -v sqlvolume:/var/opt/mssql -d --rm --name mssql mcr.microsoft.com/mssql/server:2022-latest
```

## Stopping SQL Server in Docker

```powershell
docker stop mssql
```

## Setting the connection string to secret manager

```powershell
$sa_password = "[SA PASSWORD HERE]"
dotnet user-secrets set "ConnectionStrings:GameStoreConnectionString" "Server=localhost; Database=GameStore; User Id=sa; Password=$sa_password; TrustServerCertificate=True"
```

## Add dotnet ef tool

```powershell
 dotnet tool install --global dotnet-ef
```

## Run database migrations

```powershell
 dotnet ef database update
```

## Setting the Azure Storage connection string to secret manager

```powershell
$storage_connection_string: "[STORAGE CONNECTION STRING HERE]" or storage_connection_string:"[STORAGE CONNECTION STRING HERE]" (if you are using ) Zsh
dotnet user-secrets set "ConnectionStrings:AzureStorage" $storage_connection_string
```

## See your User Secrets

```powershell
dotnet user-secrets list
```
