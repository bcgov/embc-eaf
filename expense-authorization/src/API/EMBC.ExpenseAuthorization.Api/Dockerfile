FROM registry.access.redhat.com/ubi8/dotnet-50-runtime AS base
WORKDIR /opt/app-root/app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["EMBC.ExpenseAuthorization.Api.csproj", "."]
RUN dotnet restore "./EMBC.ExpenseAuthorization.Api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "EMBC.ExpenseAuthorization.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EMBC.ExpenseAuthorization.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /opt/app-root/app
COPY --from=publish /app/publish .
COPY --from=build EMBC.ExpenseAuthorization.Api.xml .

ENTRYPOINT ["sh","-c","dotnet EMBC.ExpenseAuthorization.Api.dll"]
