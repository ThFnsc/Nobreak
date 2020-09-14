# Introdução
Esse app foi feito para analisar o estado e manter um histórico de estados de um Nobreak da TS Shara à partir da porta Serial. Mais especificamente o modelo "UPS Senoidal Universal 2200VA". Porém imagino que funcione com outros também.

# Tecnologias
Foi usado ASP.NET Core juntamente com o Entity Framework Core.
O banco de dados configurado nas migrações incluídas foi o MySQL

# Configurações

## Variáveis de ambiente

```
SETx Nobreak_ConnectionStrings:Default     "server=localhost;port=3306;database=nobreak;user=root;password=root"
SETx Nobreak_AppSettings:SerialPort        "COM3"
SETx Nobreak_AppSettings:BauldRate         "9600"
SETx Nobreak_AppSettings:RecaptchaSiteKey  "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
SETx Nobreak_AppSettings:RecaptchaSecret   "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
```

### Padrões

`BauldRate` quando não definido será usado 9600
`SerialPort` quando não definido será usado a última porta listada pelo método `SerialPort.GetPortNames()`
`RecaptchaSiteKey` ou `RecaptchaSecret` quando não definido não incluirá o ReCAPTCHA na tela de login

# [Exemplo em produção](https://camilla.thiagofnsc.dev)