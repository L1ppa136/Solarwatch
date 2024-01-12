# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy all files
COPY . .

# Restore dependencies and publish
RUN dotnet restore
RUN dotnet publish ./SolarWatch.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "SolarWatch.dll"]

ENV ASPNETCORE_URLS=http://+:80