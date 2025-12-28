FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src/LinkDotNet.Blog.Web
COPY ./ /
RUN dotnet restore "LinkDotNet.Blog.Web.csproj"
RUN dotnet build "LinkDotNet.Blog.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LinkDotNet.Blog.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LinkDotNet.Blog.Web.dll"]