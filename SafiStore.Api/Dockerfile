# Dockerfile - multi-stage for .NET 8 API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY *.sln . 2>/dev/null || true
COPY "SafiStore.Api/*.csproj" ./SafiStore.Api/
RUN dotnet restore "SafiStore.Api/SafiStore.Api.csproj"

# copy everything else and build
COPY . ./
WORKDIR /src/SafiStore.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS="http://+:10000"
COPY --from=build /app/publish ./

# create user for security
RUN addgroup --system appgroup && adduser --system appuser && chown -R appuser:appgroup /app
USER appuser

EXPOSE 10000
ENTRYPOINT ["dotnet", "SafiStore.Api.dll"]
