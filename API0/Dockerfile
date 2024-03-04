#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

ARG APP_ENV
ARG PONG_URL
ENV EXTERNAL_API_URL=$PONG_URL

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=$APP_ENV
WORKDIR /src
COPY ["API0/API0.csproj", "API0/"]
RUN dotnet restore "./API0/./API0.csproj"
COPY . .
WORKDIR "/src/API0"
RUN dotnet build "./API0.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./API0.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API0.dll"]