#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

# we do not need to expose any ports because the API is only available on the 
# internal network and accessed via the reverse proxy from the UI.
#EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
# copy each project
COPY ["EMBC.ExpenseAuthorization.Api/EMBC.ExpenseAuthorization.Api.csproj", "EMBC.ExpenseAuthorization.Api/"]
COPY ["EMBC.Tests.Unit.ExpenseAuthorization.Api/EMBC.Tests.Unit.ExpenseAuthorization.Api.csproj", "EMBC.Tests.Unit.ExpenseAuthorization.Api/"]
COPY ["EMBC.ExpenseAuthorization.sln", "."]
RUN dotnet restore .
COPY . .
RUN dotnet test -c Release

FROM build AS publish
RUN dotnet publish "EMBC.ExpenseAuthorization.Api/EMBC.ExpenseAuthorization.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EMBC.ExpenseAuthorization.Api.dll"]
