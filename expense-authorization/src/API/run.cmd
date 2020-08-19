SETLOCAL

dotnet restore
dotnet build
pushd EMBC.ExpenseAuthorization.Api\bin\Debug\netcoreapp3.1\

SET ASPNETCORE_ENVIRONMENT=Development
dotnet EMBC.ExpenseAuthorization.Api.dll
