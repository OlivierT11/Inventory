FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .
RUN dotnet restore Inventory.csproj
RUN dotnet publish Inventory.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

# Add WEBSITES_PORT=8080 in the Azure App Service configuration too.
# Azure Portal > backend Web App > Settings > Environment variables > App settings tab > + Add.
EXPOSE 8080

ENTRYPOINT ["dotnet", "Inventory.dll"]