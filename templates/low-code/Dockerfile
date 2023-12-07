FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ADD . .
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "FewBox.Template.Service.dll"]