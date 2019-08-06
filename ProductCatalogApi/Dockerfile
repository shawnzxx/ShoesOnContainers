# Build Stage
FROM mcr.microsoft.com/dotnet/core/sdk AS build-env
WORKDIR /api
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -o /publish --configuration Release

#Runtime Image Stage
FROM mcr.microsoft.com/dotnet/core/aspnet
WORKDIR /publish
COPY --from=build-env /publish .
ENTRYPOINT ["dotnet", "ProductCatalogApi.dll"]