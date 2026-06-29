# FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/sdk:9.0 AS build
# Replace with the zeus code artificat
WORKDIR /src

COPY nuget.config .
COPY TraineeManagement.Api/*.csproj TraineeManagement.Api/
COPY TraineeManagement.Data/*.csproj TraineeManagement.Data/
COPY TraineeManagement.Contracts/*.csproj TraineeManagement.Contracts/
COPY TraineeManagement.Messaging/*.csproj TraineeManagement.Messaging/

RUN dotnet restore TraineeManagement.Api/TraineeManagement.Api.csproj

COPY . .
RUN dotnet publish TraineeManagement.Api/TraineeManagement.Api.csproj -c Release -o /app/build

# FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
FROM  docker-registry-002.zeuslearning.com/zeuslearning/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "TraineeManagement.Api.dll"]
