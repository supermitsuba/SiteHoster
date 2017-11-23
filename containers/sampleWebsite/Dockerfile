FROM microsoft/dotnet:2.0.3-sdk

RUN mkdir /var/www
RUN mkdir /var/www/web
WORKDIR /var/www/web
COPY WebsiteManager/ /var/www/web

WORKDIR /var/www/web/WebsiteManager

RUN  dotnet restore
RUN dotnet publish -c Release -o out

EXPOSE 5000

CMD ["dotnet","run"]