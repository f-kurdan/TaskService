FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5018

ENV ASPNETCORE_URLS=http://+:5018

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/TaskService.Api/TaskService.csproj", "src/TaskService.Api/"]
RUN dotnet restore "src/TaskService.Api/TaskService.csproj"
COPY . .
WORKDIR "/src/src/TaskService.Api"
RUN dotnet build "TaskService.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "TaskService.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskService.dll"]
