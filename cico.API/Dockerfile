FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY cico.API/*.csproj cico.API/
COPY cico.Application/*.csproj cico.Application/
COPY cico.Domain/*.csproj cico.Domain/
COPY cico.Infrastructure/*.csproj cico.Infrastructure/
RUN dotnet restore cico.API/cico.API.csproj

COPY . .
RUN dotnet publish cico.API/cico.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "cico.API.dll"]
