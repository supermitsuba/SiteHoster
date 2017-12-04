FROM supermitsuba/microsoft-dotnet:2.0.3-sdk

#RUN apt update && apt install -y 
#RUN apt update && apt install -y git
#RUN git clone https://github.com/supermitsuba/SiteHoster.git /var/www/web
#   VOLUME [ "/Users/supermitsuba/git/SiteHoster/api/WebsiteManager/" ]
COPY ./out /var/www/web
COPY ./nginx.conf /var/www/web/nginx.conf

WORKDIR /var/www/web
RUN ls /var/www/web
#RUN  dotnet restore
#RUN dotnet publish -c Release -o out

EXPOSE 5000

#  --server.urls http://0.0.0.0:5000
CMD ["dotnet", "DotnetManager.dll", "--server.urls", "http://0.0.0.0:5000"]