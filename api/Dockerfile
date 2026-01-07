# Build Stage
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution file and central props
COPY TemporalWarehouse.slnx .
COPY Directory.Packages.props .

# Copy API project files
COPY TemporalWarehouse.Api/TemporalWarehouse.Api.csproj ./TemporalWarehouse.Api/

# Restore dependencies
RUN dotnet restore "./TemporalWarehouse.Api/TemporalWarehouse.Api.csproj"

# Copy the rest of the code
COPY . .

# Publish
WORKDIR /src/TemporalWarehouse.Api
RUN dotnet publish -c Release -o /app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TemporalWarehouse.Api.dll"]


