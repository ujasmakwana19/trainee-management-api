FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/sdk:9.0 AS build
# Replace with the zeus code artificat
WORKDIR /src

COPY nuget.config .
COPY TraineeManagement.Contracts/*.csproj TraineeManagement.Contracts/
COPY TraineeManagement.Data/*.csproj TraineeManagement.Data/
COPY TrainingDirectory.Api/*.csproj TrainingDirectory.Api/

RUN dotnet restore TrainingDirectory.Api/TrainingDirectory.Api.csproj

COPY . .
RUN dotnet publish TrainingDirectory.Api/TrainingDirectory.Api.csproj -c Release -o /app/build

FROM  docker-registry-002.zeuslearning.com/zeuslearning/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "TrainingDirectory.Api.dll"]
