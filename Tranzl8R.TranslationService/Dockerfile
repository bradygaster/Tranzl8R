FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app
EXPOSE ${ORLEANS_SILO_PORT}
EXPOSE ${ORLEANS_GATEWAY_PORT}

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tranzl8R.TranslationService/Tranzl8R.TranslationService.csproj", "Tranzl8R.TranslationService/"]
RUN dotnet restore "Tranzl8R.TranslationService/Tranzl8R.TranslationService.csproj"
COPY . .
WORKDIR "/src/Tranzl8R.TranslationService"
RUN dotnet build "Tranzl8R.TranslationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tranzl8R.TranslationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tranzl8R.TranslationService.dll"]