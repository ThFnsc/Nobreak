FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS publish
WORKDIR /src
COPY Nobreak.sln .
COPY src/Nobreak/Nobreak.csproj src/Nobreak/
COPY src/Nobreak.Infra/Nobreak.Infra.csproj src/Nobreak.Infra/
COPY tests/Nobreak.Tests/Nobreak.Tests.csproj tests/Nobreak.Tests/
RUN dotnet restore
COPY . .
RUN dotnet test
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet Nobreak.dll