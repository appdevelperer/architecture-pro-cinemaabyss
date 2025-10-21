# src/microservices/proxy/Dockerfile
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# WORKDIR /app
# EXPOSE 8000

# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /src
# COPY ["cinema.apigateway.csproj", "./"]
# RUN dotnet restore "./cinema.apigateway.csproj"
# COPY . .
# RUN dotnet publish "cinema.apigateway.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=build /app/publish .
# ENV ASPNETCORE_URLS=http://+:8000
# ENTRYPOINT ["dotnet", "cinema.apigateway.dll","--urls", "http://+:8000"]



# Stage 1: сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Stage 2: финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8400
ENV ASPNETCORE_URLS=http://+:8400
ENTRYPOINT ["dotnet", "cinema.apigateway.dll","--urls", "http://+:8400"]
