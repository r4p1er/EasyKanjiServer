FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

COPY *.sln .
COPY EasyKanjiServer/*.csproj ./EasyKanjiServer/
RUN dotnet restore

COPY EasyKanjiServer/. ./EasyKanjiServer/
WORKDIR /source/EasyKanjiServer
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "EasyKanjiServer.dll"]