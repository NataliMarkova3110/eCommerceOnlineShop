FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["eCommerceOnlineShop.IdentityServer/eCommerceOnlineShop.IdentityServer.csproj", "eCommerceOnlineShop.IdentityServer/"]
RUN dotnet restore "eCommerceOnlineShop.IdentityServer/eCommerceOnlineShop.IdentityServer.csproj"
COPY . .
WORKDIR "/src/eCommerceOnlineShop.IdentityServer"
RUN dotnet build "eCommerceOnlineShop.IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "eCommerceOnlineShop.IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eCommerceOnlineShop.IdentityServer.dll"] 