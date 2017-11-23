FROM microsoft/dotnet:2.0.3-sdk

RUN apt update && apt install -y git
RUN git clone https://github.com/supermitsuba/SiteHoster.git /var/www/web
#   VOLUME [ "/Users/supermitsuba/git/SiteHoster/api/WebsiteManager/" ]
#   ADD . /var/www/web

WORKDIR /var/www/web/api/WebsiteManager
RUN ls /var/www/web
RUN  dotnet restore
RUN dotnet publish -c Release -o out

EXPOSE 6000

CMD ["dotnet","run"]