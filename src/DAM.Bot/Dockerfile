#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DAM.Bot/DAM.Bot.csproj", "DAM.Bot/"]
COPY ["DAM.Data.EntityFramework/DAM.Data.EntityFramework.csproj", "DAM.Data.EntityFramework/"]
COPY ["DAM.Core/DAM.Core.csproj", "DAM.Core/"]
COPY ["DAM.Domain/DAM.Domain.csproj", "DAM.Domain/"]
COPY ["DAM.Database/DAM.Database.csproj", "DAM.Database/"]
RUN dotnet restore "DAM.Bot/DAM.Bot.csproj"
COPY . .
WORKDIR "/src/DAM.Bot"
RUN dotnet build "DAM.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DAM.Bot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DAM.Bot.dll"]