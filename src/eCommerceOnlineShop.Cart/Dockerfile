FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["eCommerceOnlineShop.Cart/eCommerceOnlineShop.Cart.csproj", "eCommerceOnlineShop.Cart/"]
COPY ["eCommerceOnlineShop.MessageBroker/eCommerceOnlineShop.MessageBroker.csproj", "eCommerceOnlineShop.MessageBroker/"]
RUN dotnet restore "eCommerceOnlineShop.Cart/eCommerceOnlineShop.Cart.csproj"
COPY . .
WORKDIR "/src/eCommerceOnlineShop.Cart"
RUN dotnet build "eCommerceOnlineShop.Cart.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "eCommerceOnlineShop.Cart.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eCommerceOnlineShop.Cart.dll"] 