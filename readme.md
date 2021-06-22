# Introdução
Esse app foi feito para analisar o estado e manter um histórico de estados de um Nobreak da TS Shara à partir da porta Serial. Mais especificamente o modelo "UPS Senoidal Universal 2200VA". Porém imagino que funcione com outros também.

# Tecnologias
Foi usado ASP.NET Core juntamente com o Entity Framework Core.
O banco de dados configurado nas migrações incluídas foi o MySQL

# Configurações

## Variáveis de ambiente

```
SETx Nobreak_ConnectionStrings:Default                "server=localhost;port=3306;database=nobreak;user=root;password=root"
SETx Nobreak_AppSettings:SerialPort                   "COM3"
SETx Nobreak_AppSettings:BauldRate                    "9600"
SETx Nobreak_AppSettings:RecaptchaSiteKey             "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
SETx Nobreak_AppSettings:RecaptchaSecret              "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
SETx Nobreak_AppSettings:RunMigrationsOnStartup       "true"
SETx Nobreak_AppSettings:RequireLoginToDownloadValues "true"
```

### Padrões

`BauldRate` quando não definido será usado 9600
`SerialPort` quando não definido será usado a última porta listada pelo método `SerialPort.GetPortNames()`
`RecaptchaSiteKey` ou `RecaptchaSecret` quando não definido não incluirá o ReCAPTCHA na tela de login

## Login
Um usuário será adicionado automaticamente quando for feito um login e não houver nenhum usuário cadastrado no banco de dados

# Docker

## Docker compose

Configuração sugerida

```
version: '3'

services:
  nobreak:
    image: thiagofnsc/nobreak
    restart: always
    volumes: 
      - ./keys:/root/.aspnet/DataProtection-Keys
    environment:
      Nobreak_ConnectionStrings__Default: server=mysql;port=3306;database=nobreak;user=root;password=Spvkfm7Cdj6Bv46KHF7KQ6R
      Nobreak_ConnectionStrings__Redis: redis,password=n5kU2u7naxNHYVT49UA5DjQ
      #Nobreak_AppSettings__RecaptchaSecret: 
      #Nobreak_AppSettings__RecaptchaSiteKey: 
      Nobreak_AppSettings__RunMigrationsOnStartup: "true"
      Nobreak_AppSettings__RequireLoginToDownloadValues: "false"
      Nobreak_AppSettings__BauldRate: 9600
      #Nobreak_AppSettings__SerialPort: #Manual override
    depends_on: 
      redis:
        condition: service_healthy
      mysql:
        condition: service_healthy
    ports: 
      - 3000:80
    #devices: 
      #- /dev/ttyACM0
    networks: 
      - db

  mysql:
    image: mysql
    command: --default-authentication-plugin=mysql_native_password
    restart: always
    volumes:
      - "./mysql_data:/var/lib/mysql"
    environment:
      MYSQL_ROOT_PASSWORD: Spvkfm7Cdj6Bv46KHF7KQ6R
    healthcheck:
      test: ["CMD", "mysqladmin" ,"ping", "-h", "localhost"]
      timeout: 1s
      interval: 1s
      retries: 30
    networks:
      - db

  redis:
    image: redis:alpine
    restart: always
    command: redis-server --requirepass n5kU2u7naxNHYVT49UA5DjQ
    networks:
      - db
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      timeout: 1s
      interval: 1s
      retries: 30

networks:
  db:
    driver: bridge
```

# [Exemplo em produção](https://nobreak.thiagofnsc.dev)