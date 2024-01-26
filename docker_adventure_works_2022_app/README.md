
# Docker environment for development

We use docker compose to ease development process by providing, all 3 stores required for ECapex run during development
- *Postgres* used here as an event store through the librarie [Martendb](https://martendb.io)

## Pre-requisite

You need have docker installed on your machine. We recommand for Windows the use of [Docker desktop](https://hub.docker.com/editions/community/docker-ce-desktop-windows)

Minimum recommanded versions:
- docker engine : 20.10.13
- docker compose : v2.3.3
- docker desktop (if used): 4.6.1 

## Create & destroy environment

From the currenct directory `docker`.

To create the environment :

`docker compose up -d --build --always-recreate-deps --force-recreate -V --remove-orphans` 

To destroy the environment :

`docker compose rm -s -f -v`

Scripts `start.sh` and `terminate.sh` run the two commands above.

You can access the datbase by accessing the adress "localhost,1440" using the following:
* System: SQLServer
