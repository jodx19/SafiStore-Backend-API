# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy everything from the repository root
COPY . .

# Restore and Publish the specific project
# Adjusting to the subfolder structure where the .csproj is located
RUN dotnet restore "SafiStore.Api/SafiStore.Api.csproj"
RUN dotnet publish "SafiStore.Api/SafiStore.Api.csproj" -c Release -o /app

# Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Expose the default port (Render will use this)
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "SafiStore.Api.dll"]
