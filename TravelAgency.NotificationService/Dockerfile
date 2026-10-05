# TravelAgency.NotificationService/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["TravelAgency.NotificationService/TravelAgency.NotificationService.csproj", "TravelAgency.NotificationService/"]
COPY ["TravelAgency.Contracts/TravelAgency.Contracts.csproj", "TravelAgency.Contracts/"]

RUN dotnet restore "TravelAgency.NotificationService/TravelAgency.NotificationService.csproj"

COPY . .

WORKDIR "/src/TravelAgency.NotificationService"
RUN dotnet build "TravelAgency.NotificationService.csproj" -c Release -o /app/build
RUN dotnet publish "TravelAgency.NotificationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TravelAgency.NotificationService.dll"]