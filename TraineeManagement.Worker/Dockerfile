FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/sdk:9.0 AS build
# Replace with the zeus code artificat
WORKDIR /src

COPY nuget.config .
COPY TraineeManagement.Data/*.csproj TraineeManagement.Data/
COPY TraineeManagement.Messaging/*.csproj TraineeManagement.Messaging/
COPY TraineeManagement.Worker/*.csproj TraineeManagement.Worker/

RUN dotnet restore TraineeManagement.Worker/TraineeManagement.Worker.csproj

COPY . .
RUN dotnet publish TraineeManagement.Worker/TraineeManagement.Worker.csproj -c Release -o /app/build

FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/sdk:9.0 AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "TraineeManagement.Worker.dll"]
