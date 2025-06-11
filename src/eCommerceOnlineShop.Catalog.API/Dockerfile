FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["eCommerceOnlineShop.Catalog.API/eCommerceOnlineShop.Catalog.API.csproj", "eCommerceOnlineShop.Catalog.API/"]
COPY ["eCommerceOnlineShop.Catalog.Core/eCommerceOnlineShop.Catalog.Core.csproj", "eCommerceOnlineShop.Catalog.Core/"]
COPY ["eCommerceOnlineShop.Catalog.BLL/eCommerceOnlineShop.Catalog.BLL.csproj", "eCommerceOnlineShop.Catalog.BLL/"]
COPY ["eCommerceOnlineShop.Catalog.DAL/eCommerceOnlineShop.Catalog.DAL.csproj", "eCommerceOnlineShop.Catalog.DAL/"]
COPY ["eCommerceOnlineShop.MessageBroker/eCommerceOnlineShop.MessageBroker.csproj", "eCommerceOnlineShop.MessageBroker/"]
RUN dotnet restore "eCommerceOnlineShop.Catalog.API/eCommerceOnlineShop.Catalog.API.csproj"
COPY . .
WORKDIR "/src/eCommerceOnlineShop.Catalog.API"
RUN dotnet build "eCommerceOnlineShop.Catalog.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "eCommerceOnlineShop.Catalog.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eCommerceOnlineShop.Catalog.API.dll"] 