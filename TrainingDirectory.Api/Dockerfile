FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Replace with the zeus code artificat
WORKDIR /src

COPY TrainingDirectory.Api/*.csproj TrainingDirectory.Api/
COPY TraineeManagement.Data/*.csproj TraineeManagement.Data/
COPY TraineeManagement.Contracts/*.csproj TraineeManagement.Contracts/

RUN dotnet restore TrainingDirectory.Api/TrainingDirectory.Api.csproj

COPY . .
RUN dotnet publish TrainingDirectory.Api/TrainingDirectory.Api.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "TrainingDirectory.Api.dll"]
