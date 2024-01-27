all:

migrations:
	cd src/Apotheosis.Infrastructure && dotnet ef database update	

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

up-debug:
	docker-compose -f docker-compose.yml -f docker-compose.debug.yml up -d

up-debug-a:
	docker-compose -f docker-compose.yml -f docker-compose.debug.yml up

rebuild-debug:
	docker-compose -f docker-compose.yml -f docker-compose.debug.yml build

start-debug: rebuild-debug up-debug

start-debug-a: rebuild-debug up-debug-a