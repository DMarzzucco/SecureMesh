.PHONY: infra workers infra-w systems system_wg clean purge all logs status rebuild

# stand up all infrstructure (Rabbitmq and Database)
infra:
	docker-compose up db db_hangfire rabbitmq redis 

# stand up workers
workers:
	docker-compose up worker1 worker2 worker3 worker4 

# stand up infrastructure and workers
infra-w:
	docker-compose up db db_hangfire rabbitmq redis worker1 worker2 worker3 worker4 

#stand up all APIs
systems:
	docker-compose up user auth hangfire

#stand up system without gateway
system_wg:
	docker-compose up db db_hangfire rabbitmq redis worker1 worker2 worker3 worker4 

# clean all volumes
clean:
	docker-compose down --volumes --rmi all

# purge the build
purge:
	docker builder prune -a -f

all: infra workers infra-w systems system_wg

# Get All Servers Logs
logs:
	docker-compose logs -f

# get one server logs
logs-%:
	docker-compose logs -f $*

# get status docker
status:
	docker-compose ps

# stand up all system
rebuild:
	docker-compose up --build -d