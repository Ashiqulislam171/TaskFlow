
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["TaskFlow.csproj", "./"]
RUN dotnet restore "TaskFlow.csproj"

COPY . .
RUN dotnet publish "TaskFlow.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["sh", "-c", "dotnet TaskFlow.dll --urls http://0.0.0.0:${PORT:-10000}"]
