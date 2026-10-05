# TravelAgency.BookingService/Dockerfile

# ---------- Стадия сборки ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копируем .csproj обоих проектов (сервиса и его зависимости)
COPY ["TravelAgency.BookingService/TravelAgency.BookingService.csproj", "TravelAgency.BookingService/"]
COPY ["TravelAgency.Contracts/TravelAgency.Contracts.csproj", "TravelAgency.Contracts/"]

# Восстанавливаем зависимости
RUN dotnet restore "TravelAgency.BookingService/TravelAgency.BookingService.csproj"

# Копируем весь исходный код
COPY . .

# Собираем и публикуем
WORKDIR "/src/TravelAgency.BookingService"
RUN dotnet build "TravelAgency.BookingService.csproj" -c Release -o /app/build
RUN dotnet publish "TravelAgency.BookingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------- Финальная стадия ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TravelAgency.BookingService.dll"]