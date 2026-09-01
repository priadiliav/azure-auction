FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Auction.Functions/Auction.Functions.csproj", "src/Auction.Functions/"]
COPY ["src/Auction.Infrastructure/Auction.Infrastructure.csproj", "src/Auction.Infrastructure/"]
COPY ["src/Auction.Domain/Auction.Domain.csproj", "src/Auction.Domain/"]
COPY ["src/Auction.Application/Auction.Application.csproj", "src/Auction.Application/"]
COPY ["src/Auction.Contracts/Auction.Contracts.csproj", "src/Auction.Contracts/"]
RUN dotnet restore "src/Auction.Functions/Auction.Functions.csproj"
COPY . .
WORKDIR "/src/src/Auction.Functions"
RUN dotnet build "./Auction.Functions.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Auction.Functions.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
