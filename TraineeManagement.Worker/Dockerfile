FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Replace with the zeus code artificat
WORKDIR /src

COPY TraineeManagement.Worker/*.csproj TraineeManagement.Worker/
COPY TraineeManagement.Data/*.csproj TraineeManagement.Data/
COPY TraineeManagement.Messaging/*.csproj TraineeManagement.Messaging/

RUN dotnet restore TraineeManagement.Worker/TraineeManagement.Worker.csproj

COPY . .
RUN dotnet publish TraineeManagement.Worker/TraineeManagement.Worker.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "TraineeManagement.Worker.dll"]
