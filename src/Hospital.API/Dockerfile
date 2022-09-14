FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 433

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Hospital.API/Hospital.API.csproj", "src/Hospital.API/"]
RUN dotnet restore "src/Hospital.API/Hospital.API.csproj"
COPY . .
RUN dotnet build "src/Hospital.API/Hospital.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Hospital.API/Hospital.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hospital.API.dll"]