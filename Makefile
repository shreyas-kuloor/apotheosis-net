all:

up:
	docker-compose up -d

up-a:
	docker-compose up
	
rebuild:
	docker-compose build

start: rebuild up

start-a: rebuild up-a

stop:
	docker-compose stop

down:
	docker-compose down