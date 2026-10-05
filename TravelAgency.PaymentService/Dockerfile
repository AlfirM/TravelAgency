# TravelAgency.PaymentService/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["TravelAgency.PaymentService/TravelAgency.PaymentService.csproj", "TravelAgency.PaymentService/"]
COPY ["TravelAgency.Contracts/TravelAgency.Contracts.csproj", "TravelAgency.Contracts/"]

RUN dotnet restore "TravelAgency.PaymentService/TravelAgency.PaymentService.csproj"

COPY . .

WORKDIR "/src/TravelAgency.PaymentService"
RUN dotnet build "TravelAgency.PaymentService.csproj" -c Release -o /app/build
RUN dotnet publish "TravelAgency.PaymentService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Worker Service использует runtime, а не aspnet
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TravelAgency.PaymentService.dll"]