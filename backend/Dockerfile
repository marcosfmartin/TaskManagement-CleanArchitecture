FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["TaskManagement.API/TaskManagement.API.csproj", "TaskManagement.API/"]
COPY ["TaskManagement.Application/TaskManagement.Application.csproj", "TaskManagement.Application/"]
COPY ["TaskManagement.Domain/TaskManagement.Domain.csproj", "TaskManagement.Domain/"]
COPY ["TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj", "TaskManagement.Infrastructure/"]

RUN dotnet restore "TaskManagement.API/TaskManagement.API.csproj"

COPY . .
WORKDIR "/src/TaskManagement.API"
RUN dotnet publish "TaskManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskManagement.API.dll"]