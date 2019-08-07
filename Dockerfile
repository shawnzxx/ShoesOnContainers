# Build Stage
FROM mcr.microsoft.com/dotnet/core/sdk AS builder
WORKDIR /sln
COPY ./ShoesOnContainers.sln ./

# Copy csproj and restore as distinct layers
COPY ./src/Web/WebMvc/WebMvc.csproj ./src/Web/WebMvc/WebMvc.csproj
COPY ./src/Services/Catalog/Catalog.Application/Catalog.Application.csproj ./src/Services/Catalog/Catalog.Application/Catalog.Application.csproj
COPY ./src/Services/Catalog/Catalog.Domain/Catalog.Domain.csproj ./src/Services/Catalog/Catalog.Domain/Catalog.Domain.csproj
COPY ./src/Services/Catalog/Catalog.Infra/Catalog.Infra.csproj ./src/Services/Catalog/Catalog.Infra/Catalog.Infra.csproj
RUN dotnet restore

COPY ./src ./src
RUN dotnet build -c Release --no-restore
RUN dotnet publish "./src/Services/Catalog/Catalog.Application/Catalog.Application.csproj" -c Release -o "../../../../dist" --no-restore

#Runtime Image Stage
FROM mcr.microsoft.com/dotnet/core/aspnet
WORKDIR /app
COPY --from=builder /sln/dist .
ENTRYPOINT ["dotnet", "Catalog.Application.dll"]