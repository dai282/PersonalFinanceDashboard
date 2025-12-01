# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["PersonalFinance.API/PersonalFinance.API.csproj", "PersonalFinance.API/"]
COPY ["PersonalFinance.Core/PersonalFinance.Core.csproj", "PersonalFinance.Core/"]
COPY ["PersonalFinance.Infrastructure/PersonalFinance.Infrastructure.csproj", "PersonalFinance.Infrastructure/"]
COPY ["PersonalFinance.ServiceDefaults/PersonalFinance.ServiceDefaults.csproj", "PersonalFinance.ServiceDefaults/"]

# Restore
RUN dotnet restore "PersonalFinance.API/PersonalFinance.API.csproj"

# Copy everything
COPY . .

# Build
WORKDIR "/src/PersonalFinance.API"
RUN dotnet build "PersonalFinance.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "PersonalFinance.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PersonalFinance.API.dll"]