# #See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# # Container we use for final publish
# FROM mcr.microsoft.com/dotnet/core/aspnet:8.0-buster-slim AS base
# WORKDIR /app
# EXPOSE 8082

# # Build container
# FROM mcr.microsoft.com/dotnet/core/sdk:8.0-buster AS build

# # Copy the code into the container
# WORKDIR /.
# COPY ["./Cinema.Events/Cinema.Events.csproj", "Cinema.Events/"]

# # NuGet restore
# RUN dotnet restore "Cinema.Events/Cinema.Events.csproj"
# COPY ["./Cinema.Events/", "Cinema.Events/"]

# # Build the API
# WORKDIR "Cinema.Events"
# RUN dotnet build "Cinema.Events.csproj" -c Release -o /app/build

# # Publish it
# FROM build AS publish
# RUN dotnet publish "Cinema.Events.csproj" -c Release -o /app/publish

# # Make the final image for publishing
# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Cinema.Events.dll"]


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
EXPOSE 8082
ENV ASPNETCORE_URLS=http://+:8082
ENTRYPOINT ["dotnet", "Cinema.Events.dll","--urls", "http://+:8082"]