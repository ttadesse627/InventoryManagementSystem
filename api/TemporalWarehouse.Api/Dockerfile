# Build Stage

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR  /src

# Copy
COPY TemporalWarehouse.Api.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

# Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8000
EXPOSE 8000

ENTRYPOINT [ "dotnet", "TemporalWarehouse.Api.dll"]