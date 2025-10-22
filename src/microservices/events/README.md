# Cinema.Events - ASP.NET Core 8.0 Server

API спецификация для системы CinemaAbyss, включающая монолит и микросервисы.

Система CinemaAbyss представляет собой платформу для управления фильмами, пользователями, платежами и подписками.
Архитектура системы включает монолитное приложение и выделенные микросервисы.


## Upgrade NuGet Packages

NuGet packages get frequently updated.

To upgrade this solution to the latest version of all NuGet packages, use the dotnet-outdated tool.


Install dotnet-outdated tool:

```
dotnet tool install --global dotnet-outdated-tool
```

Upgrade only to new minor versions of packages

```
dotnet outdated --upgrade --version-lock Major
```

Upgrade to all new versions of packages (more likely to include breaking API changes)

```
dotnet outdated --upgrade
```


## Run

Linux/OS X:

```
sh build.sh
```

Windows:

```
build.bat
```
## Run in Docker

```
cd ./Cinema.Events
docker build -t cinema.events .
docker run -p 5000:8080 cinema.events
```
