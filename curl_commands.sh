##SETUP##
# !!ONLY DO THIS ONCE!! for creating the docker network
docker network create --subnet=172.18.0.0/16 pizza_drones 

# for building drone and dispatcher containers
docker build -t dispatcher -f DroneDispatcher/Dockerfile . && docker build -t drone -f DroneSimulator/Dockerfile .

# for running the dispatcher container
docker run -it --rm -p 4000:4000 -e ASPNETCORE_HTTP_PORT=4000 -e ASPNETCORE_URLS=http://+:4000 --name dispatcher dispatcher --net pizza_drones --ip 172.18.0.0

# for running a drone container
docker run -it --rm -p 5000:5000 -p 5001:5001 -e ASPNETCORE_HTTP_PORT=5000 -e ASPNETCORE_URLS=http://+:5001 --name drone drone --net pizza_drones --ip 172.18.0.1

##SENDING DELIVERY##
curl -X POST localhost:4000/dispatcher/add_order -H 'Content-type: application/json' -H 'Accept: application/json' -d '{ "orderId": "TESTmrtyuafwdnrusdku"}'


##TODO DRONE REGISTRATION##
# when front end tells dispatcher to register a drone:
curl -X POST localhost:4000/dispatcher/register \
-H 'Content-type: application/json' \
-H 'Accept: application/json' \
-d '{ "badgeNumber": "76b5b3ce-305b-4baf-a949-115fcaac1cf6", "ipAddress": "http://localhost:5001"}'

# when dispatcher tells drone to be initialized:
curl -X POST localhost:5001/drone/initregistration \
-H 'Content-type: application/json' \
-H 'Accept: application/json' \
-d '{ \
     "badgeNumber": "76b5b3ce-305b-4baf-a949-115fcaac1cf6", \
     "ipAddress": "http://localhost:443", \
     "homeLocation": { \
         "latitude": 0, \
         "longitude": 0 \
     }, \
     "dispatcherUrl": "http://localhost:5000/dispatcher" \
 }'
