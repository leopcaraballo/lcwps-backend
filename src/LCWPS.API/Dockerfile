FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/LCWPS.API/LCWPS.API.csproj", "src/LCWPS.API/"]
COPY ["src/LCWPS.Application/LCWPS.Application.csproj", "src/LCWPS.Application/"]
COPY ["src/LCWPS.Domain/LCWPS.Domain.csproj", "src/LCWPS.Domain/"]
COPY ["src/LCWPS.Infrastructure/LCWPS.Infrastructure.csproj", "src/LCWPS.Infrastructure/"]
RUN dotnet restore "src/LCWPS.API/LCWPS.API.csproj"

COPY . .
RUN dotnet publish "src/LCWPS.API/LCWPS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LCWPS.API.dll"]
