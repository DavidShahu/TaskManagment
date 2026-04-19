FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

ENV NUGET_FALLBACK_PACKAGES=""

COPY TaskManagment.sln .
COPY Domain/*.csproj Domain/
COPY Application/*.csproj Application/
COPY Infrastructure/*.csproj Infrastructure/
COPY TaskManagment/*.csproj TaskManagment/
COPY UnitTests/*.csproj UnitTests/
COPY NuGet.config .

RUN dotnet restore TaskManagment.sln

COPY . .

# Let publish do its own restore — don't use --no-restore
RUN dotnet publish TaskManagment/TaskManagment.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskManagment.dll"]