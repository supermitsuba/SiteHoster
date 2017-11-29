FROM supermitsuba/microsoft-dotnet:2.0.3-sdk

#RUN apt update && apt install -y 
RUN curl -sSL https://get.docker.com | sh
#RUN apt update && apt install -y git
#RUN git clone https://github.com/supermitsuba/SiteHoster.git /var/www/web
#   VOLUME [ "/Users/supermitsuba/git/SiteHoster/api/WebsiteManager/" ]
COPY ./out /var/www/web

WORKDIR /var/www/web
RUN ls /var/www/web
#RUN  dotnet restore
#RUN dotnet publish -c Release -o out

EXPOSE 5000

#  --server.urls http://0.0.0.0:5000
CMD ["dotnet", "DockerManager.dll", "--server.urls", "http://0.0.0.0:5000"]