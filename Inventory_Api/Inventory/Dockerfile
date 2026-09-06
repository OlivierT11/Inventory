FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY Inventory.csproj ./
RUN dotnet restore ./Inventory.csproj

COPY . .
RUN dotnet publish ./Inventory.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

# Mettre dans l'image docker le resultat du build .NET, c-a-d les dll, non les fichiers source .cs
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Inventory.dll"]