FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY GruersShop.Data.Common/GruersShop.Data.Common.csproj GruersShop.Data.Common/
COPY GruersShop.Data.Models/GruersShop.Data.Models.csproj GruersShop.Data.Models/
COPY GruersShop.Data/GruersShop.Data.csproj GruersShop.Data/
COPY GruersShop.Services.Automapping/GruersShop.Services.Automapping.csproj GruersShop.Services.Automapping/
COPY GruersShop.Services.Common/GruersShop.Services.Common.csproj GruersShop.Services.Common/
COPY GruersShop.Services.Core/GruersShop.Services.Core.csproj GruersShop.Services.Core/
COPY GruersShop.Web.Infrastructure/GruersShop.Web.Infrastructure.csproj GruersShop.Web.Infrastructure/
COPY GruersShop.Web/GruersShop.Web.csproj GruersShop.Web/

RUN dotnet restore "GruersShop.Web/GruersShop.Web.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "GruersShop.Web/GruersShop.Web.csproj" \
    -c Release \
    -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "GruersShop.Web.dll"]