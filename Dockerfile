# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for optimized layer caching
COPY ["src/FamilyManagement.Domain/FamilyManagement.Domain.csproj", "src/FamilyManagement.Domain/"]
COPY ["src/FamilyManagement.Application/FamilyManagement.Application.csproj", "src/FamilyManagement.Application/"]
COPY ["src/FamilyManagement.Infrastructure/FamilyManagement.Infrastructure.csproj", "src/FamilyManagement.Infrastructure/"]
COPY ["src/FamilyManagement.ServiceDefaults/FamilyManagement.ServiceDefaults.csproj", "src/FamilyManagement.ServiceDefaults/"]
COPY ["src/FamilyManagement.UI.Shared/FamilyManagement.UI.Shared.csproj", "src/FamilyManagement.UI.Shared/"]
COPY ["src/FamilyManagement.Web/FamilyManagement.Web.csproj", "src/FamilyManagement.Web/"]
COPY ["src/FamilyManagement.Api/FamilyManagement.Api.csproj", "src/FamilyManagement.Api/"]

# Restore dependencies
RUN dotnet restore "src/FamilyManagement.Api/FamilyManagement.Api.csproj"

# Copy source code and build/publish
COPY ["src/FamilyManagement.Domain/", "src/FamilyManagement.Domain/"]
COPY ["src/FamilyManagement.Application/", "src/FamilyManagement.Application/"]
COPY ["src/FamilyManagement.Infrastructure/", "src/FamilyManagement.Infrastructure/"]
COPY ["src/FamilyManagement.ServiceDefaults/", "src/FamilyManagement.ServiceDefaults/"]
COPY ["src/FamilyManagement.UI.Shared/", "src/FamilyManagement.UI.Shared/"]
COPY ["src/FamilyManagement.Web/", "src/FamilyManagement.Web/"]
COPY ["src/FamilyManagement.Api/", "src/FamilyManagement.Api/"]

# Publish Blazor WebAssembly frontend (processes asset fingerprint placeholders and importmaps)
WORKDIR "/src/src/FamilyManagement.Web"
RUN dotnet publish "FamilyManagement.Web.csproj" -c Release -o /app/publish_web /p:UseAppHost=false

# Publish Backend API
WORKDIR "/src/src/FamilyManagement.Api"
RUN dotnet publish "FamilyManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Merge fully processed Blazor WebAssembly assets into API wwwroot
RUN cp -r /app/publish_web/wwwroot/* /app/publish/wwwroot/

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Create data directory and set proper ownership for non-root user
RUN mkdir -p /app/data /app/receipts_storage && chown -R $APP_UID:$APP_UID /app

COPY --from=build --chown=$APP_UID:$APP_UID /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "FamilyManagement.Api.dll"]
