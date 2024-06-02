FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
COPY DominosDriverHustleComp.csproj /app/
RUN dotnet restore /app/DominosDriverHustleComp.csproj
COPY . /app
RUN cd /app \
 && dotnet build DominosDriverHustleComp.csproj -c $BUILD_CONFIGURATION -o /app/build \
 && dotnet publish DominosDriverHustleComp.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
COPY --from=build /app/publish /app
EXPOSE 80
EXPOSE 443
WORKDIR /app
ENTRYPOINT ["dotnet", "DominosDriverHustleComp.dll"]
