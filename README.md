API calls that are there.

docker
======
POST: api/docker/container/run/{serviceName}
POST: api/docker/container/build/{serviceName}
POST: api/docker/container/start/{serviceName}
POST: api/docker/container/stop/{serviceName}
POST: api/docker/container/remove/{serviceName}

discovery-service
=================
GET:  api/website/
GET:  api/website/{name}
POST: api/website/
PUT:  api/website/{name}
DEL:  api/website/{name}

Nginx
=====
POST: api/nginx/container/create
POST: api/nginx/config/{serviceName}
POST: api/nginx/stop
POST: api/nginx/reload
POST: api/nginx/start