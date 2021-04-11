#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Nobreak/Nobreak.csproj", "src/Nobreak/"]
COPY ["src/Nobreak.Infra/Nobreak.Infra.csproj", "src/Nobreak.Infra/"]
RUN dotnet restore "src/Nobreak/Nobreak.csproj"
COPY . .
WORKDIR "/src/src/Nobreak"
RUN dotnet build "Nobreak.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nobreak.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nobreak.dll"]