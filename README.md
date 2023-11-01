# Manager_Tasks

Gerenciamento de tarefas
Esse sistema foi desenvolvido para fazer o gerenciamento de tarefas.

Caso já possua o banco de dados, será necessário editar a senha e usuário no arquivo: appsettings

Se não tiver, copie essas linhas de código abaixo para um arquivo com nome e extensão docker-compose.yml

version: '3.1'

services:

db:
image: postgres
container_name: postgres
restart: always
volumes: - db:/var/lib/postgresql/data
environment:
POSTGRES_PASSWORD: postgres
PGDATA: /data/postgres
ports: - "5432:5432"

volumes:
db:
driver: local

Com o docker ativo na sua máquina. Vá até o local que criou o arquivo docker-compose.yml e abra o terminal e rode o seguinte comando
para criar um container.

docker-compose up -d

Com essa configuração não será necessário editar o appsettings.

Para criar um usuário no sistema será necessário entrar na seguinte URL para acessar Swagger
https://localhost:7020/swagger/index.html
