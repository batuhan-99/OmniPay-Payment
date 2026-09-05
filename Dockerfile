# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY OmniPay.API.sln ./
COPY OmniPay.API/OmniPay.API.csproj OmniPay.API/
COPY OmniPay.Tests/OmniPay.Tests.csproj OmniPay.Tests/
COPY src/OmniPay.Application/OmniPay.Application.csproj src/OmniPay.Application/
COPY src/OmniPay.Domain/OmniPay.Domain.csproj src/OmniPay.Domain/
COPY src/OmniPay.Infrastructure/OmniPay.Infrastructure.csproj src/OmniPay.Infrastructure/

RUN dotnet restore OmniPay.API.sln

COPY . .
RUN dotnet publish OmniPay.API/OmniPay.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OmniPay.API.dll"]
