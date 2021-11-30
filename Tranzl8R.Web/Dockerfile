FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 11111
EXPOSE 30000
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tranzl8R.Web/Tranzl8R.Web.csproj", "Tranzl8R.Web/"]
COPY ["Tranzl8R.Grains.Interfaces/Tranzl8R.Grains.Interfaces.csproj", "Tranzl8R.Grains.Interfaces/"]
COPY ["Tranzl8R.Infrastructure/Tranzl8R.Infrastructure.csproj", "Tranzl8R.Infrastructure/"]
COPY ["Tranzl8R.Grains/Tranzl8R.Grains.csproj", "Tranzl8R.Grains/"]
RUN dotnet restore "Tranzl8R.Web/Tranzl8R.Web.csproj"
COPY . .
WORKDIR "/src/Tranzl8R.Web"
RUN dotnet build "Tranzl8R.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tranzl8R.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tranzl8R.Web.dll"]